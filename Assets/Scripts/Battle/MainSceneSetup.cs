using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace AstrobloxRPG
{
    /// <summary>
    /// Main scene setup for the battle system
    /// </summary>
    public class MainSceneSetup : MonoBehaviour
    {
        [Header("Battle System Components")]
        [SerializeField] private BattleManager battleManager;
        [SerializeField] private BattleUIManager battleUIManager;
        [SerializeField] private BattleSceneController battleSceneController;
        [SerializeField] private BattleSystemTest battleSystemTest;
        
        [Header("UI Components")]
        [SerializeField] private UIPrefabCreator uiPrefabCreator;
        [SerializeField] private Canvas mainCanvas;
        [SerializeField] private RectTransform playerUIContainer;
        [SerializeField] private RectTransform enemyUIContainer;
        [SerializeField] private RectTransform battleResultPanel;
        
        private void Awake()
        {
            // Create all required components if they don't exist
            CreateRequiredComponents();
        }
        
        private void Start()
        {
            // Initialize the battle system
            InitializeBattleSystem();
            
            // Run the battle test
            if (battleSystemTest != null)
            {
                battleSystemTest.RunBattleTest();
            }
        }
        
        private void CreateRequiredComponents()
        {
            // Create main canvas if it doesn't exist
            if (mainCanvas == null)
            {
                GameObject canvasObj = new GameObject("MainCanvas");
                mainCanvas = canvasObj.AddComponent<Canvas>();
                mainCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
                canvasObj.AddComponent<UnityEngine.UI.CanvasScaler>();
                canvasObj.AddComponent<UnityEngine.UI.GraphicRaycaster>();
            }
            
            // Create UI containers if they don't exist
            if (playerUIContainer == null)
            {
                GameObject containerObj = new GameObject("PlayerUIContainer");
                containerObj.transform.SetParent(mainCanvas.transform);
                playerUIContainer = containerObj.AddComponent<RectTransform>();
                playerUIContainer.anchorMin = new Vector2(0, 0.5f);
                playerUIContainer.anchorMax = new Vector2(0.5f, 1);
                playerUIContainer.pivot = new Vector2(0.5f, 0.5f);
                playerUIContainer.sizeDelta = Vector2.zero;
                playerUIContainer.anchoredPosition = Vector2.zero;
            }
            
            if (enemyUIContainer == null)
            {
                GameObject containerObj = new GameObject("EnemyUIContainer");
                containerObj.transform.SetParent(mainCanvas.transform);
                enemyUIContainer = containerObj.AddComponent<RectTransform>();
                enemyUIContainer.anchorMin = new Vector2(0.5f, 0.5f);
                enemyUIContainer.anchorMax = new Vector2(1, 1);
                enemyUIContainer.pivot = new Vector2(0.5f, 0.5f);
                enemyUIContainer.sizeDelta = Vector2.zero;
                enemyUIContainer.anchoredPosition = Vector2.zero;
            }
            
            // Create battle result panel if it doesn't exist
            if (battleResultPanel == null)
            {
                GameObject panelObj = new GameObject("BattleResultPanel");
                panelObj.transform.SetParent(mainCanvas.transform);
                battleResultPanel = panelObj.AddComponent<RectTransform>();
                
                // Updated to use center of screen with larger fixed size
                battleResultPanel.anchorMin = new Vector2(0.5f, 0.5f); 
                battleResultPanel.anchorMax = new Vector2(0.5f, 0.5f);
                battleResultPanel.pivot = new Vector2(0.5f, 0.5f);
                battleResultPanel.sizeDelta = new Vector2(500, 250); // Increased size
                battleResultPanel.anchoredPosition = Vector2.zero; // Center of screen
                
                // Add panel background with border
                UnityEngine.UI.Image panelImage = panelObj.AddComponent<UnityEngine.UI.Image>();
                panelImage.color = new Color(0.1f, 0.1f, 0.3f, 0.95f);
                panelImage.raycastTarget = true; // Make sure it blocks raycasts
                
                // Find the UIPrefabCreator for rounded sprites
                UIPrefabCreator prefabCreator = FindObjectOfType<UIPrefabCreator>();
                if (prefabCreator != null)
                {
                    panelImage.sprite = prefabCreator.CreateRoundedRectSprite(12);
                }
                
                // Add result text
                GameObject textObj = new GameObject("ResultText");
                textObj.transform.SetParent(panelObj.transform);
                RectTransform textRect = textObj.AddComponent<RectTransform>();
                textRect.anchorMin = new Vector2(0, 0);
                textRect.anchorMax = new Vector2(1, 1);
                textRect.sizeDelta = Vector2.zero; // Fill the panel
                textRect.anchoredPosition = Vector2.zero;
                
                TMPro.TextMeshProUGUI resultText = textObj.AddComponent<TMPro.TextMeshProUGUI>();
                resultText.alignment = TMPro.TextAlignmentOptions.Center;
                resultText.fontSize = 72; // Larger font size
                resultText.font = Resources.Load<TMPro.TMP_FontAsset>("Fonts/LiberationSans SDF"); // Use default TMPro font
                resultText.color = Color.white;
                resultText.fontStyle = TMPro.FontStyles.Bold;
                resultText.enableWordWrapping = false; // Prevent word wrapping
                resultText.overflowMode = TMPro.TextOverflowModes.Overflow; // Allow text to overflow
                
                // Add outline for better visibility
                resultText.outlineWidth = 0.3f; // Thicker outline
                resultText.outlineColor = Color.black;
                
                // Set some placeholder text to verify it's working
                resultText.text = "BATTLE RESULT";
                
                // Hide panel initially
                panelObj.SetActive(false);
            }
            
            // Create UI prefab creator if it doesn't exist
            if (uiPrefabCreator == null)
            {
                GameObject creatorObj = new GameObject("UIPrefabCreator");
                uiPrefabCreator = creatorObj.AddComponent<UIPrefabCreator>();
            }
            
            // Create battle manager if it doesn't exist
            if (battleManager == null)
            {
                GameObject managerObj = new GameObject("BattleManager");
                battleManager = managerObj.AddComponent<BattleManager>();
            }
            
            // Create battle UI manager if it doesn't exist
            if (battleUIManager == null)
            {
                GameObject uiManagerObj = new GameObject("BattleUIManager");
                battleUIManager = uiManagerObj.AddComponent<BattleUIManager>();
                
                // Set references
                var uiManagerType = battleUIManager.GetType();
                SetPrivateField(battleUIManager, "playerUIContainer", playerUIContainer);
                SetPrivateField(battleUIManager, "enemyUIContainer", enemyUIContainer);
                SetPrivateField(battleUIManager, "battleResultPanel", battleResultPanel.gameObject);
                SetPrivateField(battleUIManager, "battleResultText", battleResultPanel.GetComponentInChildren<TMPro.TextMeshProUGUI>());
                SetPrivateField(battleUIManager, "agentUIPrefab", uiPrefabCreator.GetComponent<UIPrefabCreator>().GetAgentUIPrefab());
            }
            
            // Create player and enemy parties if they don't exist
            GameObject playerPartyObj = GameObject.Find("PlayerParty");
            if (playerPartyObj == null)
            {
                playerPartyObj = new GameObject("PlayerParty");
                Party playerParty = playerPartyObj.AddComponent<Party>();
                SetPrivateField(playerParty, "partyName", "Players");
                SetPrivateField(playerParty, "isPlayerParty", true);
            }
            
            GameObject enemyPartyObj = GameObject.Find("EnemyParty");
            if (enemyPartyObj == null)
            {
                enemyPartyObj = new GameObject("EnemyParty");
                Party enemyParty = enemyPartyObj.AddComponent<Party>();
                SetPrivateField(enemyParty, "partyName", "Enemies");
                SetPrivateField(enemyParty, "isPlayerParty", false);
            }
            
            // Set party references in battle manager
            SetPrivateField(battleManager, "playerParty", playerPartyObj.GetComponent<Party>());
            SetPrivateField(battleManager, "enemyParty", enemyPartyObj.GetComponent<Party>());
            
            // Create battle scene controller if it doesn't exist
            if (battleSceneController == null)
            {
                GameObject controllerObj = new GameObject("BattleSceneController");
                battleSceneController = controllerObj.AddComponent<BattleSceneController>();
                
                // Set references
                SetPrivateField(battleSceneController, "battleManager", battleManager);
                SetPrivateField(battleSceneController, "uiManager", battleUIManager);
            }
            
            // Create battle system test if it doesn't exist
            if (battleSystemTest == null)
            {
                GameObject testObj = new GameObject("BattleSystemTest");
                battleSystemTest = testObj.AddComponent<BattleSystemTest>();
                
                // Set references
                SetPrivateField(battleSystemTest, "battleSceneController", battleSceneController);
                SetPrivateField(battleSystemTest, "battleManager", battleManager);
                SetPrivateField(battleSystemTest, "uiManager", battleUIManager);
                
                // Create default configs
                DefaultAgentConfigs configs = ScriptableObject.CreateInstance<DefaultAgentConfigs>();
                SetPrivateField(battleSystemTest, "defaultConfigs", configs);
            }
        }
        
        private void InitializeBattleSystem()
        {
            // Initialize UI manager with battle manager
            battleUIManager.Initialize(battleManager);
        }
        
        private void SetPrivateField(object obj, string fieldName, object value)
        {
            var field = obj.GetType().GetField(fieldName, 
                System.Reflection.BindingFlags.Instance | 
                System.Reflection.BindingFlags.NonPublic | 
                System.Reflection.BindingFlags.Public);
            
            if (field != null)
            {
                field.SetValue(obj, value);
            }
            else
            {
                Debug.LogWarning($"Field {fieldName} not found on {obj.GetType().Name}");
            }
        }

        // Add this method to create the configuration UI
        private void CreateBattleConfigUI()
        {
            // Create configuration panel
            GameObject configPanel = new GameObject("ConfigPanel");
            configPanel.transform.SetParent(mainCanvas.transform, false);
            
            RectTransform panelRect = configPanel.AddComponent<RectTransform>();
            panelRect.anchorMin = new Vector2(0.2f, 0.2f);
            panelRect.anchorMax = new Vector2(0.8f, 0.8f);
            panelRect.sizeDelta = Vector2.zero;
            
            Image panelImage = configPanel.AddComponent<Image>();
            panelImage.color = new Color(0.1f, 0.1f, 0.2f, 0.9f);
            
            // Create title
            GameObject titleObj = new GameObject("Title");
            titleObj.transform.SetParent(configPanel.transform, false);
            
            RectTransform titleRect = titleObj.AddComponent<RectTransform>();
            titleRect.anchorMin = new Vector2(0.5f, 0.9f);
            titleRect.anchorMax = new Vector2(0.5f, 0.95f);
            titleRect.sizeDelta = new Vector2(400, 50);
            titleRect.anchoredPosition = Vector2.zero;
            
            TextMeshProUGUI titleText = titleObj.AddComponent<TextMeshProUGUI>();
            titleText.text = "BATTLE CONFIGURATION";
            titleText.fontSize = 36;
            titleText.color = Color.white;
            titleText.alignment = TextAlignmentOptions.Center;
            
            // Create player party slider
            GameObject playerSliderObj = CreateSlider(configPanel, "PlayerPartySlider", 
                new Vector2(0.5f, 0.7f), "Player Party Size: 3");
            
            // Create enemy party slider
            GameObject enemySliderObj = CreateSlider(configPanel, "EnemyPartySlider", 
                new Vector2(0.5f, 0.5f), "Enemy Party Size: 3");
            
            // Create game speed slider
            GameObject speedSliderObj = CreateSlider(configPanel, "GameSpeedSlider", 
                new Vector2(0.5f, 0.3f), "Game Speed: 1.0x");
            
            // Create start button
            GameObject startButtonObj = new GameObject("StartButton");
            startButtonObj.transform.SetParent(configPanel.transform, false);
            
            RectTransform startButtonRect = startButtonObj.AddComponent<RectTransform>();
            startButtonRect.anchorMin = new Vector2(0.5f, 0.1f);
            startButtonRect.anchorMax = new Vector2(0.5f, 0.2f);
            startButtonRect.sizeDelta = new Vector2(200, 60);
            startButtonRect.anchoredPosition = Vector2.zero;
            
            Image buttonImage = startButtonObj.AddComponent<Image>();
            buttonImage.color = new Color(0.2f, 0.6f, 0.2f, 1f);
            
            Button startButton = startButtonObj.AddComponent<Button>();
            startButton.targetGraphic = buttonImage;
            
            GameObject buttonTextObj = new GameObject("Text");
            buttonTextObj.transform.SetParent(startButtonObj.transform, false);
            
            RectTransform buttonTextRect = buttonTextObj.AddComponent<RectTransform>();
            buttonTextRect.anchorMin = Vector2.zero;
            buttonTextRect.anchorMax = Vector2.one;
            buttonTextRect.sizeDelta = Vector2.zero;
            
            TextMeshProUGUI buttonText = buttonTextObj.AddComponent<TextMeshProUGUI>();
            buttonText.text = "START BATTLE";
            buttonText.fontSize = 24;
            buttonText.color = Color.white;
            buttonText.alignment = TextAlignmentOptions.Center;
            
            // Get reference to config manager
            BattleConfigManager configManager = FindObjectOfType<BattleConfigManager>();
            if (configManager == null)
            {
                GameObject configManagerObj = new GameObject("BattleConfigManager");
                configManager = configManagerObj.AddComponent<BattleConfigManager>();
            }
            
            // Set references to UI elements
            SetPrivateField(configManager, "configPanel", configPanel);
            SetPrivateField(configManager, "playerPartySlider", playerSliderObj.GetComponent<Slider>());
            SetPrivateField(configManager, "enemyPartySlider", enemySliderObj.GetComponent<Slider>());
            SetPrivateField(configManager, "gameSpeedSlider", speedSliderObj.GetComponent<Slider>());
            SetPrivateField(configManager, "playerPartyText", playerSliderObj.GetComponentInChildren<TextMeshProUGUI>());
            SetPrivateField(configManager, "enemyPartyText", enemySliderObj.GetComponentInChildren<TextMeshProUGUI>());
            SetPrivateField(configManager, "gameSpeedText", speedSliderObj.GetComponentInChildren<TextMeshProUGUI>());
            SetPrivateField(configManager, "startBattleButton", startButton);
        }

        // Helper method to create slider with label
        private GameObject CreateSlider(GameObject parent, string name, Vector2 position, string labelText)
        {
            GameObject sliderObj = new GameObject(name);
            sliderObj.transform.SetParent(parent.transform, false);
            
            RectTransform sliderRect = sliderObj.AddComponent<RectTransform>();
            sliderRect.anchorMin = new Vector2(0.5f, position.y - 0.05f);
            sliderRect.anchorMax = new Vector2(0.5f, position.y + 0.05f);
            sliderRect.sizeDelta = new Vector2(400, 50);
            sliderRect.anchoredPosition = Vector2.zero;
            
            Slider slider = sliderObj.AddComponent<Slider>();
            
            // Create background
            GameObject bgObj = new GameObject("Background");
            bgObj.transform.SetParent(sliderObj.transform, false);
            
            RectTransform bgRect = bgObj.AddComponent<RectTransform>();
            bgRect.anchorMin = Vector2.zero;
            bgRect.anchorMax = new Vector2(1f, 0.5f);
            bgRect.sizeDelta = Vector2.zero;
            
            Image bgImage = bgObj.AddComponent<Image>();
            bgImage.color = new Color(0.2f, 0.2f, 0.2f, 1f);
            
            // Create fill area
            GameObject fillAreaObj = new GameObject("Fill Area");
            fillAreaObj.transform.SetParent(sliderObj.transform, false);
            
            RectTransform fillAreaRect = fillAreaObj.AddComponent<RectTransform>();
            fillAreaRect.anchorMin = Vector2.zero;
            fillAreaRect.anchorMax = new Vector2(1f, 0.5f);
            fillAreaRect.sizeDelta = new Vector2(-20, 0);
            fillAreaRect.anchoredPosition = new Vector2(-5, 0);
            
            // Create fill
            GameObject fillObj = new GameObject("Fill");
            fillObj.transform.SetParent(fillAreaObj.transform, false);
            
            RectTransform fillRect = fillObj.AddComponent<RectTransform>();
            fillRect.anchorMin = Vector2.zero;
            fillRect.anchorMax = Vector2.one;
            fillRect.sizeDelta = Vector2.zero;
            
            Image fillImage = fillObj.AddComponent<Image>();
            fillImage.color = new Color(0.4f, 0.6f, 1f, 1f);
            
            // Create handle area
            GameObject handleAreaObj = new GameObject("Handle Slide Area");
            handleAreaObj.transform.SetParent(sliderObj.transform, false);
            
            RectTransform handleAreaRect = handleAreaObj.AddComponent<RectTransform>();
            handleAreaRect.anchorMin = Vector2.zero;
            handleAreaRect.anchorMax = Vector2.one;
            handleAreaRect.sizeDelta = new Vector2(-20, 0);
            handleAreaRect.anchoredPosition = Vector2.zero;
            
            // Create handle
            GameObject handleObj = new GameObject("Handle");
            handleObj.transform.SetParent(handleAreaObj.transform, false);
            
            RectTransform handleRect = handleObj.AddComponent<RectTransform>();
            handleRect.sizeDelta = new Vector2(20, 30);
            
            Image handleImage = handleObj.AddComponent<Image>();
            handleImage.color = Color.white;
            
            // Set slider properties
            slider.fillRect = fillRect;
            slider.handleRect = handleRect;
            slider.targetGraphic = handleImage;
            slider.direction = Slider.Direction.LeftToRight;
            
            // Create label
            GameObject labelObj = new GameObject("Label");
            labelObj.transform.SetParent(sliderObj.transform, false);
            
            RectTransform labelRect = labelObj.AddComponent<RectTransform>();
            labelRect.anchorMin = new Vector2(0.5f, 0.6f);
            labelRect.anchorMax = new Vector2(0.5f, 1f);
            labelRect.sizeDelta = new Vector2(400, 30);
            labelRect.anchoredPosition = Vector2.zero;
            
            TextMeshProUGUI labelTextComponent = labelObj.AddComponent<TextMeshProUGUI>();
            labelTextComponent.text = labelText;
            labelTextComponent.fontSize = 24;
            labelTextComponent.color = Color.white;
            labelTextComponent.alignment = TextAlignmentOptions.Center;
            
            return sliderObj;
        }
    }
}
