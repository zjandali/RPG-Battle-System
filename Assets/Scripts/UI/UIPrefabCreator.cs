using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace AstrobloxRPG
{
    /// <summary>
    /// Prefab creator for UI elements
    /// </summary>
    public class UIPrefabCreator : MonoBehaviour
    {
        [Header("Agent UI Prefab")]
        [SerializeField] private GameObject agentUIPrefabRoot;
        [SerializeField] private RectTransform healthBarRect;
        [SerializeField] private RectTransform actionBarRect;
        [SerializeField] private RectTransform effectsContainerRect;
        
        public GameObject GetAgentUIPrefab() => agentUIPrefabRoot;
        public GameObject GetEffectUIPrefab() => effectUIPrefabRoot;
        
        [Header("Effect UI Prefab")]
        [SerializeField] private GameObject effectUIPrefabRoot;
        
        private void Awake()
        {
            // Create prefabs if they don't exist
            CreateAgentUIPrefab();
            CreateEffectUIPrefab();
        }
        
        private void CreateAgentUIPrefab()
        {
            if (agentUIPrefabRoot != null) return;
            
            // Create effect UI prefab first
            CreateEffectUIPrefab();
            
            // Create agent UI prefab
            agentUIPrefabRoot = new GameObject("AgentUIPrefab");
            agentUIPrefabRoot.AddComponent<RectTransform>();
            
            // Add background panel
            GameObject bgPanel = new GameObject("Background");
            bgPanel.transform.SetParent(agentUIPrefabRoot.transform);
            RectTransform bgRect = bgPanel.AddComponent<RectTransform>();
            bgRect.anchorMin = Vector2.zero;
            bgRect.anchorMax = Vector2.one;
            bgRect.sizeDelta = new Vector2(10, 10); // Add some padding
            bgRect.anchoredPosition = Vector2.zero;
            Image bgImage = bgPanel.AddComponent<Image>();
            bgImage.color = new Color(0.1f, 0.1f, 0.2f, 0.8f); // Semi-transparent dark blue
            bgImage.sprite = CreateRoundedRectSprite(10); // Create a rounded rectangle sprite
            
            // Add Canvas Group for fade effects
            agentUIPrefabRoot.AddComponent<CanvasGroup>();
            
            // Add AgentUI component
            AgentUI agentUI = agentUIPrefabRoot.AddComponent<AgentUI>();
            
            // Create name text with better styling
            GameObject nameTextObj = new GameObject("NameText");
            nameTextObj.transform.SetParent(agentUIPrefabRoot.transform);
            RectTransform nameTextRect = nameTextObj.AddComponent<RectTransform>();
            nameTextRect.anchorMin = new Vector2(0.05f, 0.91f); // Same top position
            nameTextRect.anchorMax = new Vector2(0.6f, 1f); // Reduced to make room for health text
            nameTextRect.pivot = new Vector2(0, 0.5f); // Left-aligned pivot
            nameTextRect.sizeDelta = Vector2.zero;
            nameTextRect.anchoredPosition = Vector2.zero;
            TMPro.TextMeshProUGUI nameText = nameTextObj.AddComponent<TMPro.TextMeshProUGUI>();
            nameText.alignment = TMPro.TextAlignmentOptions.Left; // Left-aligned text
            nameText.fontSize = 18;
            nameText.fontStyle = TMPro.FontStyles.Bold;
            nameText.color = Color.white;
            // Add outline for better readability
            nameText.outlineWidth = 0.2f;
            nameText.outlineColor = Color.black;
            
            // Create health bar with icon
            GameObject healthBarObj = new GameObject("HealthBar");
            healthBarObj.transform.SetParent(agentUIPrefabRoot.transform);
            healthBarRect = healthBarObj.AddComponent<RectTransform>();
            healthBarRect.anchorMin = new Vector2(0.05f, 0.8f);
            healthBarRect.anchorMax = new Vector2(0.95f, 0.9f);
            healthBarRect.sizeDelta = Vector2.zero;
            
            // Add Slider component to health bar
            UnityEngine.UI.Slider healthSlider = healthBarObj.AddComponent<UnityEngine.UI.Slider>();
            
            // Change the health background to use the full width (no icon space needed)
            GameObject healthBgObj = new GameObject("Background");
            healthBgObj.transform.SetParent(healthBarObj.transform);
            RectTransform healthBgRect = healthBgObj.AddComponent<RectTransform>();
            healthBgRect.anchorMin = new Vector2(0, 0); // Changed from 0.1f to 0 (use full width)
            healthBgRect.anchorMax = Vector2.one;
            healthBgRect.sizeDelta = Vector2.zero;
            UnityEngine.UI.Image healthBgImage = healthBgObj.AddComponent<UnityEngine.UI.Image>();
            healthBgImage.color = new Color(0.2f, 0.2f, 0.2f, 1f);
            healthBgImage.sprite = CreateRoundedRectSprite(5);
            
            // Also adjust the health fill to use the full width
            GameObject healthFillObj = new GameObject("Fill");
            healthFillObj.transform.SetParent(healthBarObj.transform);
            RectTransform healthFillRect = healthFillObj.AddComponent<RectTransform>();
            healthFillRect.anchorMin = new Vector2(0, 0); // Changed from 0.1f to 0 (use full width)
            healthFillRect.anchorMax = Vector2.one;
            healthFillRect.sizeDelta = new Vector2(-4, -4);
            UnityEngine.UI.Image healthFillImage = healthFillObj.AddComponent<UnityEngine.UI.Image>();
            healthFillImage.sprite = CreateRoundedRectSprite(4);
            healthFillImage.color = Color.green;
            
            // Create health text in top right corner
            GameObject healthTextObj = new GameObject("HealthText");
            healthTextObj.transform.SetParent(agentUIPrefabRoot.transform); // Parent to root
            RectTransform healthTextRect = healthTextObj.AddComponent<RectTransform>();
            healthTextRect.anchorMin = new Vector2(0.65f, 0.91f); // Top right corner, same height as name
            healthTextRect.anchorMax = new Vector2(0.95f, 1f);
            healthTextRect.sizeDelta = Vector2.zero;
            TMPro.TextMeshProUGUI healthText = healthTextObj.AddComponent<TMPro.TextMeshProUGUI>();
            healthText.alignment = TMPro.TextAlignmentOptions.Right; // Right-aligned text
            healthText.fontSize = 14;
            healthText.color = Color.white;
            // Add outline for better readability on any background
            healthText.fontStyle = TMPro.FontStyles.Bold;
            healthText.outlineWidth = 0.2f;
            healthText.outlineColor = Color.black;
            
            // Create action bar with better labeling
            GameObject actionBarObj = new GameObject("ActionBar");
            actionBarObj.transform.SetParent(agentUIPrefabRoot.transform);
            actionBarRect = actionBarObj.AddComponent<RectTransform>();
            actionBarRect.anchorMin = new Vector2(0, 0.7f);
            actionBarRect.anchorMax = new Vector2(1, 0.75f);
            actionBarRect.sizeDelta = Vector2.zero;
            actionBarRect.anchoredPosition = Vector2.zero;
            UnityEngine.UI.Slider actionSlider = actionBarObj.AddComponent<UnityEngine.UI.Slider>();

            // Create action bar background
            GameObject actionBgObj = new GameObject("Background");
            actionBgObj.transform.SetParent(actionBarObj.transform);
            RectTransform actionBgRect = actionBgObj.AddComponent<RectTransform>();
            actionBgRect.anchorMin = Vector2.zero;
            actionBgRect.anchorMax = Vector2.one;
            actionBgRect.sizeDelta = Vector2.zero;
            UnityEngine.UI.Image actionBgImage = actionBgObj.AddComponent<UnityEngine.UI.Image>();
            actionBgImage.color = new Color(0.2f, 0.2f, 0.2f);
            
            // Adjust the action bar fill to use the full width (no label space needed)
            GameObject actionFillObj = new GameObject("Fill");
            actionFillObj.transform.SetParent(actionBarObj.transform);
            RectTransform actionFillRect = actionFillObj.AddComponent<RectTransform>();
            actionFillRect.anchorMin = new Vector2(0, 0); // Changed from 0.15f to 0 (use full width)
            actionFillRect.anchorMax = Vector2.one;
            actionFillRect.sizeDelta = new Vector2(-2, -2); // Add margin inside
            actionFillRect.anchoredPosition = Vector2.zero;
            UnityEngine.UI.Image actionFillImage = actionFillObj.AddComponent<UnityEngine.UI.Image>();
            actionFillImage.sprite = CreateRoundedRectSprite(3);

            // Create gradient for action bar
            Gradient actionGradient = new Gradient();
            actionGradient.SetKeys(
                new GradientColorKey[] { 
                    new GradientColorKey(new Color(0.8f, 0.8f, 0.2f), 0.0f), 
                    new GradientColorKey(new Color(1f, 1f, 0.3f), 1.0f) 
                },
                new GradientAlphaKey[] { 
                    new GradientAlphaKey(1.0f, 0.0f), 
                    new GradientAlphaKey(1.0f, 1.0f) 
                }
            );

            // Apply gradient to action bar
            // You'll need to create a gradient material or textures
            // For simplicity, we'll just use a brighter yellow color
            actionFillImage.color = new Color(1f, 1f, 0.3f);
            
            // Configure action slider
            actionSlider.fillRect = actionFillRect;
            actionSlider.targetGraphic = actionFillImage;
            actionSlider.minValue = 0;
            actionSlider.maxValue = 1;
            actionSlider.value = 0;
            
            // Create effects container
            GameObject effectsContainerObj = new GameObject("EffectsContainer");
            effectsContainerObj.transform.SetParent(agentUIPrefabRoot.transform);
            effectsContainerRect = effectsContainerObj.AddComponent<RectTransform>();
            effectsContainerRect.anchorMin = new Vector2(0, 0);
            effectsContainerRect.anchorMax = new Vector2(1, 0.6f);
            effectsContainerRect.pivot = new Vector2(0.5f, 0.5f);
            effectsContainerRect.sizeDelta = Vector2.zero;
            effectsContainerRect.anchoredPosition = Vector2.zero;
            
            // Set references in AgentUI component
            agentUI.nameText = nameText;
            agentUI.healthSlider = healthSlider;
            agentUI.healthText = healthText;
            agentUI.actionSlider = actionSlider;
            agentUI.effectsContainer = effectsContainerRect;
            agentUI.effectPrefab = effectUIPrefabRoot;

            // Configure health slider
            healthSlider.fillRect = healthFillRect;
            healthSlider.targetGraphic = healthFillImage;
            healthSlider.minValue = 0;
            healthSlider.maxValue = 100;
            healthSlider.value = 100;
        }
        
        private void CreateEffectUIPrefab()
        {
            if (effectUIPrefabRoot != null) return;
            
            // Create effect UI prefab
            effectUIPrefabRoot = new GameObject("EffectUIPrefab");
            effectUIPrefabRoot.AddComponent<RectTransform>();
            
            // Add text component
            TMPro.TextMeshProUGUI effectText = effectUIPrefabRoot.AddComponent<TMPro.TextMeshProUGUI>();
            effectText.alignment = TMPro.TextAlignmentOptions.Left;
            effectText.fontSize = 12;
        }

        public Sprite CreateRoundedRectSprite(float cornerRadius)
        {
            int textureSize = 128;
            Texture2D texture = new Texture2D(textureSize, textureSize);
            
            // Fill with white pixels
            Color[] colors = new Color[textureSize * textureSize];
            for (int i = 0; i < colors.Length; i++)
            {
                colors[i] = Color.white;
            }
            
            // Create rounded corners by setting transparent pixels
            for (int y = 0; y < textureSize; y++)
            {
                for (int x = 0; x < textureSize; x++)
                {
                    // Check if this pixel is in any corner
                    bool inCorner = false;
                    
                    // Top-left corner
                    if (x < cornerRadius && y < cornerRadius)
                    {
                        float distance = Vector2.Distance(new Vector2(x, y), new Vector2(cornerRadius, cornerRadius));
                        inCorner = distance > cornerRadius;
                    }
                    // Top-right corner
                    else if (x > textureSize - cornerRadius && y < cornerRadius)
                    {
                        float distance = Vector2.Distance(new Vector2(x, y), new Vector2(textureSize - cornerRadius, cornerRadius));
                        inCorner = distance > cornerRadius;
                    }
                    // Bottom-left corner
                    else if (x < cornerRadius && y > textureSize - cornerRadius)
                    {
                        float distance = Vector2.Distance(new Vector2(x, y), new Vector2(cornerRadius, textureSize - cornerRadius));
                        inCorner = distance > cornerRadius;
                    }
                    // Bottom-right corner
                    else if (x > textureSize - cornerRadius && y > textureSize - cornerRadius)
                    {
                        float distance = Vector2.Distance(new Vector2(x, y), new Vector2(textureSize - cornerRadius, textureSize - cornerRadius));
                        inCorner = distance > cornerRadius;
                    }
                    
                    if (inCorner)
                    {
                        colors[y * textureSize + x] = Color.clear;
                    }
                }
            }
            
            texture.SetPixels(colors);
            texture.Apply();
            
            return Sprite.Create(texture, new Rect(0, 0, textureSize, textureSize), new Vector2(0.5f, 0.5f));
        }
    }
}
