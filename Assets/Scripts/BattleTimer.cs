using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace AstrobloxRPG
{
    public class BattleTimer : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI timerText;
        private float elapsedTime = 0f;
        private bool isRunning = false;
        private Canvas parentCanvas;
        private float timeScale = 1.0f; // Default time scale

        void Awake()
        {
            // Only log a warning if timerText is null
            if (timerText == null)
            {
                Debug.LogWarning("Timer text component not assigned yet. Will need to be set manually.");
            }
            
            // Make sure we're in a canvas
            parentCanvas = GetComponentInParent<Canvas>();
            if (parentCanvas == null)
            {
                Debug.LogError("Timer is not attached to a Canvas!");
            }
            
            Debug.Log("BattleTimer initialized");
        }

        void Update()
        {
            if (isRunning)
            {
                // Scale time by the battle speed
                elapsedTime += Time.deltaTime * timeScale;
                UpdateDisplay();
            }
        }

        public void StartTimer()
        {
            elapsedTime = 0f;
            isRunning = true;
            UpdateDisplay();
            Debug.Log("Timer started with speed scale: " + timeScale);
        }

        public void StopTimer()
        {
            isRunning = false;
            UpdateDisplay();
            Debug.Log("Timer stopped - Final time: " + timerText.text);
        }

        // Add method to set the time scale based on battle speed
        public void SetTimeScale(float battleSpeed)
        {
            timeScale = battleSpeed;
            Debug.Log($"Timer speed set to {timeScale}x");
        }

        private void UpdateDisplay()
        {
            int minutes = Mathf.FloorToInt(elapsedTime / 60f);
            int seconds = Mathf.FloorToInt(elapsedTime % 60f);
            int milliseconds = Mathf.FloorToInt((elapsedTime * 100f) % 100f);
            timerText.text = $"{minutes:00}:{seconds:00}.{milliseconds:00}";
        }

        // Add this public method to set the timer text
        public void SetTimerText(TextMeshProUGUI text)
        {
            timerText = text;
            Debug.Log("Timer text component assigned: " + (timerText != null));
        }
    }
} 