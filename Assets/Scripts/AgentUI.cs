using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace AstrobloxRPG
{
    /// <summary>
    /// UI component for displaying agent information
    /// </summary>
    public class AgentUI : MonoBehaviour
    {
        [Header("UI Components")]
        public TextMeshProUGUI nameText;
        public Slider healthSlider;
        public TextMeshProUGUI healthText;
        public Slider actionSlider;
        public RectTransform effectsContainer;
        public GameObject effectPrefab;
        
        // Reference to the agent
        private Agent agent;
        
        // List of effect UI elements
        private List<GameObject> effectUIElements = new List<GameObject>();
        
        public void Initialize(Agent agent)
        {
            this.agent = agent;
            
            // Set initial UI values
            if (nameText != null)
            {
                nameText.text = agent.gameObject.name;
            }
            
            UpdateUI();
        }
        
        public void UpdateUI()
        {
            if (agent == null) return;
            
            // Update health - make sure we're using the exact values
            if (healthSlider != null)
            {
                healthSlider.maxValue = agent.GetMaxHealth();
                healthSlider.value = agent.GetCurrentHealth();
            }
            
            if (healthText != null)
            {
                // Don't round the values to ensure accuracy
                healthText.text = $"{agent.GetCurrentHealth():F0}/{agent.GetMaxHealth():F0}";
            }
            
            // Update action readiness
            if (actionSlider != null)
            {
                actionSlider.value = agent.ActionReadiness;
            }
            
            // Update effects
            UpdateEffectsUI();
            
            // Update visibility based on alive status
            if (!agent.IsAlive)
            {
                // Fade out the UI for dead agents
                CanvasGroup canvasGroup = GetComponent<CanvasGroup>();
                if (canvasGroup != null)
                {
                    canvasGroup.alpha = 0.5f;
                }
            }
            
            // Only animate health bar when there's a significant change
            if (healthSlider != null && 
                Mathf.Abs(healthSlider.value - agent.GetCurrentHealth()) > 0.01f)
            {
                StopAllCoroutines(); // Stop any in-progress animations
                StartCoroutine(AnimateHealthBar(agent.GetCurrentHealth()));
            }
        }
        
        private void UpdateEffectsUI()
        {
            // Clear old effect UI elements
            foreach (GameObject effectUI in effectUIElements)
            {
                Destroy(effectUI);
            }
            effectUIElements.Clear();
            
            // Get active effects from the agent
            List<Effect> activeEffects = agent.GetActiveEffects();
            
            // Skip if agent is dead or null
            if (agent == null || !agent.IsAlive)
            {
                return;
            }
            
            // Create UI elements for each effect with improved positioning
            for (int i = 0; i < activeEffects.Count; i++)
            {
                Effect effect = activeEffects[i];
                GameObject effectUI = Instantiate(effectPrefab, effectsContainer);
                
                RectTransform rectTransform = effectUI.GetComponent<RectTransform>();
                
                // For players, show effects on RIGHT side of the card
                // For enemies, show effects on LEFT side of the card
                bool isPlayer = agent is Player;
                
                // Calculate column and row for multi-column layout
                int maxPerColumn = 3; // Maximum effects per column
                int column = i / maxPerColumn; // 0 for first 3, 1 for next 3, etc.
                int row = i % maxPerColumn; // 0, 1, 2, repeat
                
                // Calculate position
                float width = 1.0f;
                float height = 0.2f; // Make each effect shorter
                float spacing = 0.1f; // Add more space between effects
                
                // Calculate X position based on column
                float xOffset = column * (width + 0.1f); // Add horizontal spacing between columns
                float xPos = isPlayer ? 1.0f + xOffset : -width - xOffset;
                
                // Adjust Y position based on row - start higher for better visibility
                float yPos = 0.9f - (row * (height + spacing));
                
                // Set anchors and position
                rectTransform.anchorMin = new Vector2(isPlayer ? 1.0f : -width - xOffset, yPos - height);
                rectTransform.anchorMax = new Vector2(isPlayer ? 1.0f + width : 0 - xOffset, yPos);
                rectTransform.pivot = new Vector2(isPlayer ? 0 : 1, 0.5f);
                rectTransform.sizeDelta = Vector2.zero;
                
                // Add background with rounded corners for better readability
                GameObject bgObj = new GameObject("Background");
                bgObj.transform.SetParent(effectUI.transform);
                bgObj.transform.SetAsFirstSibling(); // Put behind text
                
                RectTransform bgRect = bgObj.AddComponent<RectTransform>();
                bgRect.anchorMin = Vector2.zero;
                bgRect.anchorMax = Vector2.one;
                bgRect.sizeDelta = Vector2.zero;
                
                Image bgImage = bgObj.AddComponent<Image>();
                
                // Find a UIPrefabCreator instance
                UIPrefabCreator prefabCreator = FindObjectOfType<UIPrefabCreator>();
                if (prefabCreator != null)
                {
                    bgImage.sprite = prefabCreator.CreateRoundedRectSprite(8);
                }
                
                // Rest of the code for setting up the effect text and colors
                TextMeshProUGUI effectText = effectUI.GetComponent<TextMeshProUGUI>();
                if (effectText == null)
                {
                    effectText = effectUI.AddComponent<TextMeshProUGUI>();
                }
                
                // Show effect description with time remaining
                effectText.text = $"{effect.GetEffectDescription()}\n{Mathf.Round(effect.RemainingTime)}s";
                effectText.fontSize = 12; // Slightly smaller font for better fit
                effectText.alignment = TMPro.TextAlignmentOptions.Center;
                
                // Set colors based on effect type
                Color textColor = Color.white;
                Color bgColor = new Color(0, 0, 0, 0.8f); // More opaque background
                
                if (effect is BuffEffect)
                {
                    textColor = Color.white;
                    bgColor = new Color(0.2f, 0.2f, 0.4f, 0.9f); // Changed from green to dark blue
                }
                else if (effect is DebuffEffect)
                {
                    textColor = Color.white;
                    bgColor = new Color(0.7f, 0.2f, 0.2f, 0.9f);
                }
                else if (effect is DamageOverTimeEffect)
                {
                    textColor = Color.white;
                    bgColor = new Color(0.8f, 0.4f, 0.0f, 0.9f);
                }
                else if (effect is HealOverTimeEffect)
                {
                    textColor = Color.black;
                    bgColor = new Color(0.0f, 0.8f, 0.8f, 0.9f);
                }
                
                effectText.color = textColor;
                bgImage.color = bgColor;
                
                effectUIElements.Add(effectUI);
            }
        }
        
        private IEnumerator AnimateHealthBar(float targetValue)
        {
            float startValue = healthSlider.value;
            float time = 0;
            float duration = 0.3f; // Faster animation to match combat pace
            
            while (time < duration)
            {
                time += Time.deltaTime;
                float t = time / duration;
                // Use easing function for smoother animation
                t = 1f - Mathf.Pow(1f - t, 3f); // Ease out cubic
                
                healthSlider.value = Mathf.Lerp(startValue, targetValue, t);
                
                // Update the text during animation too
                if (healthText != null)
                {
                    healthText.text = $"{Mathf.Floor(healthSlider.value):F0}/{agent.GetMaxHealth():F0}";
                }
                
                yield return null;
            }
            
            // Ensure final values are exactly correct
            healthSlider.value = targetValue;
            if (healthText != null)
            {
                healthText.text = $"{targetValue:F0}/{agent.GetMaxHealth():F0}";
            }
        }
    }
}
