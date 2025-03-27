using UnityEngine;
using UnityEngine.UI;

namespace AstrobloxRPG
{
    /// <summary>
    /// Updates the action bar color based on its current value
    /// </summary>
    public class ActionBarColorUpdater : MonoBehaviour
    {
        public Image fillImage;
        public Gradient gradient;
        public Slider slider;
        
        private void Update()
        {
            if (fillImage != null && gradient != null && slider != null)
            {
                // Update color based on current slider value
                fillImage.color = gradient.Evaluate(slider.value);
                
                // Optional: Make it pulse slightly when full
                if (slider.value >= 0.99f)
                {
                    float pulse = (Mathf.Sin(Time.time * 5) + 1) * 0.1f + 0.8f;
                    fillImage.color = new Color(
                        fillImage.color.r * pulse,
                        fillImage.color.g * pulse,
                        fillImage.color.b * pulse,
                        fillImage.color.a
                    );
                }
            }
        }
    }
} 