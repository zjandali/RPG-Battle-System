using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor;

namespace AstrobloxRPG
{
    /// <summary>
    /// UI Manager for the battle system
    /// </summary>
    public class BattleUIManager : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private BattleManager battleManager;
        
        [Header("UI Containers")]
        [SerializeField] private Transform playerUIContainer;
        [SerializeField] private Transform enemyUIContainer;
        
        [Header("UI Prefabs")]
        [SerializeField] private GameObject agentUIPrefab;
        
        [Header("Battle Result UI")]
        [SerializeField] private GameObject battleResultPanel;
        [SerializeField] private TextMeshProUGUI battleResultText;
        
        [Header("Timer UI")]
        [SerializeField] private TextMeshProUGUI timerText;
        private BattleTimer battleTimer;
        
        // Dictionary to track agent UI elements
        private Dictionary<Agent, AgentUI> agentUIElements = new Dictionary<Agent, AgentUI>();
        
        // Add this field to track battle result
        private bool isPlayerVictory = false;
        
        public void Initialize(BattleManager battleManager)
        {
            this.battleManager = battleManager;
            
            CreateStandaloneTimer();
            
            // Subscribe to battle events
                battleManager.OnBattleStart += HandleBattleStart;
                battleManager.OnBattleEnd += HandleBattleEnd;
                battleManager.OnPlayerVictory += HandlePlayerVictory;
                battleManager.OnPlayerDefeat += HandlePlayerDefeat;
            
            // Reset victory state
            isPlayerVictory = false;
            
            // Other initialization code...
            
            // Find the battle result text if we haven't already
            if (battleResultText == null && battleResultPanel != null)
            {
                battleResultText = battleResultPanel.GetComponentInChildren<TMPro.TextMeshProUGUI>();
            }
            
            // Hide battle result panel initially
            if (battleResultPanel != null)
            {
                battleResultPanel.SetActive(false);
            }
            
            Debug.Log("BattleUIManager initialized");
        }
        
        private void HandleBattleStart()
        {
            if (battleTimer != null)
            {
                battleTimer.StartTimer();
                Debug.Log("Battle started, timer started");
            }
            else
            {
                Debug.LogError("Battle timer is null!");
            }
            
            // Clear existing UI elements
            ClearUIElements();
            
            // Create UI elements for all agents
            CreateAgentUIElements();
        }
        
        private void HandleBattleEnd()
        {
            if (battleTimer != null)
            {
                battleTimer.StopTimer();
                Debug.Log("Battle ended, timer stopped");
            }
            
            Debug.Log("Battle End Handler Called");

            // Ensure we have a valid reference to the text
            if (battleResultText == null && battleResultPanel != null)
            {
                battleResultText = battleResultPanel.GetComponentInChildren<TextMeshProUGUI>();
                Debug.Log("Found battle result text: " + (battleResultText != null));
            }

            // Update battle result text
            if (battleResultText != null)
            {
                // Set text with larger content
                battleResultText.text = isPlayerVictory ? "VICTORY!" : "DEFEAT!";
                battleResultText.color = isPlayerVictory ? Color.yellow : Color.red;
                
                // Ensure text is properly formatted
                battleResultText.fontSize = 72; // Larger font size
                battleResultText.alignment = TextAlignmentOptions.Center;
                battleResultText.enableAutoSizing = true; // Enable auto-sizing
                battleResultText.fontSizeMin = 36; // Minimum font size
                battleResultText.fontSizeMax = 72; // Maximum font size
                
                Debug.Log("Set result text to: " + battleResultText.text);
            }
            else
            {
                Debug.LogError("Battle result text component is missing!");
            }
            
            // Show battle result panel
            if (battleResultPanel != null)
            {
                // Reset panel position and scale before showing it
                RectTransform panelRect = battleResultPanel.GetComponent<RectTransform>();
                if (panelRect != null)
                {
                    panelRect.anchoredPosition = Vector2.zero;
                    panelRect.localScale = Vector3.one;
                    
                    // Make panel larger for better visibility
                    panelRect.sizeDelta = new Vector2(500, 250);
                }
                
                // Make sure panel is visible and on top
                battleResultPanel.transform.SetAsLastSibling();
                battleResultPanel.SetActive(true);
                
                // Add a bit of animation to make it noticeable
                StartCoroutine(AnimateVictoryPanel());
                
                Debug.Log("Battle result panel activated");
            }
            else
            {
                Debug.LogError("Battle result panel is missing!");
            }
        }
        
        private void HandlePlayerVictory()
        {
            // Set victory flag
            isPlayerVictory = true;
            
            // Update battle result text
            if (battleResultText != null)
            {
                battleResultText.text = "Victory!";
                battleResultText.color = Color.green;
            }
        }
        
        private void HandlePlayerDefeat()
        {
            // Set defeat flag
            isPlayerVictory = false;
            
            // Update battle result text
            if (battleResultText != null)
            {
                battleResultText.text = "Defeat!";
                battleResultText.color = Color.red;
            }
        }
        
        private void ClearUIElements()
        {
            // Clear dictionary
            agentUIElements.Clear();
            
            // Clear player UI container
            foreach (Transform child in playerUIContainer)
            {
                Destroy(child.gameObject);
            }
            
            // Clear enemy UI container
            foreach (Transform child in enemyUIContainer)
            {
                Destroy(child.gameObject);
            }
        }
        
        private void CreateAgentUIElements()
        {
            // Get all player agents
            List<Agent> playerAgents = battleManager.GetPlayerParty().Agents;
            
            // Get all enemy agents
            List<Agent> enemyAgents = battleManager.GetEnemyParty().Agents;
            
            // Setup agent UIs
            SetupAgentUIs();
        }
        
        private void SetupAgentUIs()
        {
            // Clear any existing UI elements
            ClearUIElements();
            
            // Get all player agents
            List<Agent> playerAgents = battleManager.GetPlayerParty().Agents;
            
            // Get all enemy agents
            List<Agent> enemyAgents = battleManager.GetEnemyParty().Agents;
            
            // Create UIs for player agents with better spacing and positioning
            for (int i = 0; i < playerAgents.Count; i++)
            {
                // Create UI for player agent
                GameObject uiObject = Instantiate(agentUIPrefab, playerUIContainer);
                
                // Position with better spacing - spread vertically across the left side
                RectTransform rect = uiObject.GetComponent<RectTransform>();
                
                // Use left side of screen with more vertical spread
                // Position from top to bottom with larger gaps
                rect.anchorMin = new Vector2(0.05f, 0.75f - (i * 0.3f)); 
                rect.anchorMax = new Vector2(0.4f, 0.97f - (i * 0.3f));
                rect.anchoredPosition = Vector2.zero;
                rect.sizeDelta = Vector2.zero;
                
                // Create background for the player
                CreateAgentBackground(uiObject, true, i);
                
                // Initialize UI with agent
                AgentUI agentUI = uiObject.GetComponent<AgentUI>();
                agentUI.Initialize(playerAgents[i]);
                
                // Add to dictionary
                agentUIElements.Add(playerAgents[i], agentUI);
            }
            
            // Create UIs for enemy agents with better spacing and positioning
            for (int i = 0; i < enemyAgents.Count; i++)
            {
                // Create UI for enemy agent
                GameObject uiObject = Instantiate(agentUIPrefab, enemyUIContainer);
                
                // Position with better spacing - spread vertically across the right side
                RectTransform rect = uiObject.GetComponent<RectTransform>();
                
                // Use right side of screen with more vertical spread
                rect.anchorMin = new Vector2(0.6f, 0.75f - (i * 0.3f));
                rect.anchorMax = new Vector2(0.95f, 0.97f - (i * 0.3f));
                rect.anchoredPosition = Vector2.zero;
                rect.sizeDelta = Vector2.zero;
                
                // Create background for the enemy
                CreateAgentBackground(uiObject, false, i);
                
                // Initialize UI with agent
                AgentUI agentUI = uiObject.GetComponent<AgentUI>();
                agentUI.Initialize(enemyAgents[i]);
                
                // Add to dictionary
                agentUIElements.Add(enemyAgents[i], agentUI);
            }
        }
        
        private void CreateAgentBackground(GameObject uiObject, bool isPlayer, int index)
        {
            // Create a background GameObject as a sibling to the UI (so it appears behind it)
            GameObject bgObj = new GameObject($"{(isPlayer ? "Player" : "Enemy")}Background_{index}");
            bgObj.transform.SetParent(uiObject.transform.parent);
            bgObj.transform.SetSiblingIndex(uiObject.transform.GetSiblingIndex());
            
            RectTransform bgRect = bgObj.AddComponent<RectTransform>();
            // Make background slightly larger than the UI element
            bgRect.anchorMin = ((RectTransform)uiObject.transform).anchorMin - new Vector2(0.02f, 0.02f);
            bgRect.anchorMax = ((RectTransform)uiObject.transform).anchorMax + new Vector2(0.02f, 0.04f);
            bgRect.anchoredPosition = Vector2.zero;
            bgRect.sizeDelta = Vector2.zero;
            
            // Add image component with a rounded background
            Image bgImage = bgObj.AddComponent<Image>();
            
            // Find the UIPrefabCreator for rounded sprites
            UIPrefabCreator prefabCreator = FindObjectOfType<UIPrefabCreator>();
            if (prefabCreator != null)
            {
                bgImage.sprite = prefabCreator.CreateRoundedRectSprite(12);
            }
            
            // Set different background colors for players vs enemies
            if (isPlayer)
            {
                bgImage.color = new Color(0.2f, 0.3f, 0.5f, 0.4f); // Blue-ish for players
            }
            else
            {
                bgImage.color = new Color(0.5f, 0.2f, 0.2f, 0.4f); // Red-ish for enemies
            }
        }
        
        private void HandleAgentDamaged(Agent agent)
        {
            // Update UI for damaged agent
            if (agentUIElements.TryGetValue(agent, out AgentUI agentUI))
            {
                agentUI.UpdateUI();
            }
        }
        
        private void HandleAgentHealed(Agent agent)
        {
            // Update UI for healed agent
            if (agentUIElements.TryGetValue(agent, out AgentUI agentUI))
            {
                agentUI.UpdateUI();
            }
        }
        
        private void HandleAgentBuffed(Agent agent)
        {
            // Update UI for buffed agent
            if (agentUIElements.TryGetValue(agent, out AgentUI agentUI))
            {
                agentUI.UpdateUI();
            }
        }
        
        private void HandleAgentDebuffed(Agent agent)
        {
            // Update UI for debuffed agent
            if (agentUIElements.TryGetValue(agent, out AgentUI agentUI))
            {
                agentUI.UpdateUI();
            }
        }
        
        private void HandleAgentDeath(Agent agent)
        {
            // Update UI for dead agent
            if (agentUIElements.TryGetValue(agent, out AgentUI agentUI))
            {
                agentUI.UpdateUI();
            }
        }
        
        private void Update()
        {
            // Update all agent UIs
            foreach (var kvp in agentUIElements)
            {
                kvp.Value.UpdateUI();
            }
        }
        
        private IEnumerator AnimateVictoryPanel()
        {
            // Get the RectTransform component
            RectTransform panelRect = battleResultPanel.GetComponent<RectTransform>();
            if (panelRect == null) yield break; // Exit if not found
            
            // Initial scale
            panelRect.localScale = Vector3.zero;
            
            // Pop-in animation
            float duration = 0.5f;
            float time = 0;
            
            while (time < duration)
            {
                time += Time.deltaTime;
                float t = time / duration;
                
                // Bounce effect - overshoot and settle
                float scale = Mathf.Sin(t * Mathf.PI * 0.5f);
                scale = 1 + 0.2f * Mathf.Sin((t - 0.3f) * 8); // Add some bounce
                scale = Mathf.Clamp(scale, 0, 1.2f);
                
                panelRect.localScale = new Vector3(scale, scale, 1);
                
                yield return null;
            }
            
            // Ensure final scale is exactly 1
            panelRect.localScale = Vector3.one;
            
            // Make the panel stay visible for a few seconds
            yield return new WaitForSeconds(3.0f);
            
            // Optional: fade out or hide the panel after showing for a while
            // battleResultPanel.SetActive(false);
        }
        
        private void ClearAgentUIElements()
        {
            // Clear dictionary
            agentUIElements.Clear();
            
            // Clear player UI container
            foreach (Transform child in playerUIContainer)
            {
                Destroy(child.gameObject);
            }
            
            // Clear enemy UI container
            foreach (Transform child in enemyUIContainer)
            {
                Destroy(child.gameObject);
            }
        }

        // Add this method to identify and remove any unwanted bottom UI elements
        private void Start()
        {
            // Existing start code...
            
            // Find and remove any unwanted progress bars at bottom of screen
            RemoveUnwantedBottomUI();
        }

        private void RemoveUnwantedBottomUI()
        {
            // Look for all images in the main canvas that might be at the bottom
            Image[] allImages = FindObjectsOfType<Image>();
            
            foreach (Image img in allImages)
            {
                // Check if this image is positioned at the bottom of the screen
                RectTransform rect = img.GetComponent<RectTransform>();
                if (rect != null)
                {
                    // If this element is at the bottom of the screen and not part of our battle UI
                    if (rect.anchorMin.y < 0.1f && rect.anchorMax.y < 0.2f && 
                        !rect.IsChildOf(playerUIContainer) && !rect.IsChildOf(enemyUIContainer) &&
                        !rect.IsChildOf(battleResultPanel.transform))
                    {
                        // This is likely our unwanted bar
                        Debug.Log("Found unwanted UI element at bottom: " + img.gameObject.name);
                        img.gameObject.SetActive(false);
                    }
                }
            }
        }

        // Also check and hide during buff application
        public void OnBuffApplied(Agent agent, BuffEffect buff)
        {
            // Update the agent UI
            if (agentUIElements.TryGetValue(agent, out AgentUI agentUI))
            {
                agentUI.UpdateUI();
            }
            
            // Remove any unwanted bottom UI elements that might appear
            RemoveUnwantedBottomUI();
        }

        // Add this method to help debug and fix text issues
        public void DebugPanel()
        {
            if (battleResultPanel != null && battleResultPanel.activeSelf)
            {
                // Get all TextMeshProUGUI components in the panel
                TextMeshProUGUI[] texts = battleResultPanel.GetComponentsInChildren<TextMeshProUGUI>(true);
                
                Debug.Log($"Panel has {texts.Length} text components");
                
                foreach (var text in texts)
                {
                    Debug.Log($"Text: '{text.text}', Size: {text.fontSize}, Color: {text.color}, Visible: {text.enabled}");
                    
                    // Force text to be visible and large
                    text.enabled = true;
                    text.fontSize = 72;
                    text.color = Color.white;
                    text.outlineWidth = 0.2f;
                    text.outlineColor = Color.black;
                }
            }
            else
            {
                Debug.Log("Panel is not active or null");
            }
        }

        private void CreateStandaloneTimer()
        {
            Debug.Log("Creating standalone timer...");
            
            // Find the main canvas
            Canvas mainCanvas = FindObjectOfType<Canvas>();
            if (mainCanvas == null)
            {
                Debug.LogError("No Canvas found in the scene to attach timer to!");
                return;
            }
            
            // Create timer container
            GameObject timerObj = new GameObject("GameTimer");
            timerObj.transform.SetParent(mainCanvas.transform, false);
            
            // Add RectTransform
            RectTransform timerRect = timerObj.AddComponent<RectTransform>();
            timerRect.anchorMin = new Vector2(0.5f, 0f);
            timerRect.anchorMax = new Vector2(0.5f, 0f);
            timerRect.pivot = new Vector2(0.5f, 0.5f);
            timerRect.sizeDelta = new Vector2(400, 80);
            timerRect.anchoredPosition = new Vector2(0, 40); // 40 pixels from bottom
            
            // Create background panel
            GameObject bgPanel = new GameObject("TimerBackground");
            bgPanel.transform.SetParent(timerObj.transform, false);
            
            RectTransform bgRect = bgPanel.AddComponent<RectTransform>();
            bgRect.anchorMin = Vector2.zero;
            bgRect.anchorMax = Vector2.one;
            bgRect.sizeDelta = Vector2.zero;
            
            Image bgImage = bgPanel.AddComponent<Image>();
            bgImage.color = new Color(0.1f, 0.1f, 0.2f, 0.95f); // Very dark blue, almost opaque
            
            // Create timer text
            GameObject textObj = new GameObject("TimerText");
            textObj.transform.SetParent(timerObj.transform, false);
            
            RectTransform textRect = textObj.AddComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.sizeDelta = Vector2.zero;
            
            TextMeshProUGUI timeText = textObj.AddComponent<TextMeshProUGUI>();
            timeText.fontSize = 48; // Very large text
            timeText.color = Color.white;
            timeText.alignment = TMPro.TextAlignmentOptions.Center;
            timeText.text = "00:00.00";
            
            // First add the BattleTimer component
            BattleTimer battleTimer = timerObj.AddComponent<BattleTimer>();
            
            // Directly assign the text component after the BattleTimer is created
            battleTimer.SetTimerText(timeText);
            
            // Get the battle speed from the battle manager and set it on the timer
            if (battleManager != null)
            {
                // This assumes that the BattleManager has a GetBattleSpeed() method
                battleTimer.SetTimeScale(battleManager.GetBattleSpeed());
            }
            
            // Store reference
            this.battleTimer = battleTimer;
            
            Debug.Log("Standalone timer created");
        }
    }
}
