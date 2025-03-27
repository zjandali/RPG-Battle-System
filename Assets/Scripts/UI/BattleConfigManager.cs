using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace AstrobloxRPG
{
    public class BattleConfigManager : MonoBehaviour
    {
        // References to UI elements
        private GameObject configPanel;
        private Slider playerPartySlider;
        private Slider enemyPartySlider;
        private Slider gameSpeedSlider;
        private TextMeshProUGUI playerPartyText;
        private TextMeshProUGUI enemyPartyText;
        private TextMeshProUGUI gameSpeedText;
        private Button startBattleButton;
        
        // References to game controllers
        private BattleSceneController battleSceneController;
        private BattleManager battleManager;
        
        // Configuration values
        private int playerPartySize = 3;
        private int enemyPartySize = 3;
        private float gameSpeed = 1.0f;
        
        public void Initialize(BattleSceneController controller, BattleManager manager)
        {
            this.battleSceneController = controller;
            this.battleManager = manager;
            
            Debug.Log("Creating battle configuration UI...");
            CreateConfigurationUI();
        }
        
        private void CreateConfigurationUI()
        {
            // Find the main canvas
            Canvas mainCanvas = FindObjectOfType<Canvas>();
            if (mainCanvas == null)
            {
                Debug.LogError("No Canvas found in the scene!");
                return;
            }
            
            // IMPORTANT: Make sure the Canvas has proper settings for UI interaction
            mainCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            
            // Add a GraphicRaycaster if missing (required for UI interaction)
            if (mainCanvas.GetComponent<GraphicRaycaster>() == null)
            {
                mainCanvas.gameObject.AddComponent<GraphicRaycaster>();
                Debug.Log("Added GraphicRaycaster to Canvas");
            }
            
            // Create a dark orange background that fills the entire screen
            GameObject backgroundObj = new GameObject("GameBackground");
            backgroundObj.transform.SetParent(mainCanvas.transform, false);
            
            // Put it at the first position so it appears behind everything else
            backgroundObj.transform.SetSiblingIndex(0);
            
            RectTransform bgRect = backgroundObj.AddComponent<RectTransform>();
            bgRect.anchorMin = Vector2.zero;
            bgRect.anchorMax = Vector2.one;
            bgRect.sizeDelta = Vector2.zero;
            bgRect.anchoredPosition = Vector2.zero;
            
            Image bgImage = backgroundObj.AddComponent<Image>();
            // Dark orange color
            bgImage.color = new Color(0.7f, 0.3f, 0.0f, 1.0f);
            
            // Ensure EventSystem exists in the scene (required for UI input)
            if (FindObjectOfType<UnityEngine.EventSystems.EventSystem>() == null)
            {
                GameObject eventSystem = new GameObject("EventSystem");
                eventSystem.AddComponent<UnityEngine.EventSystems.EventSystem>();
                eventSystem.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
                Debug.Log("Created EventSystem - required for UI interaction");
            }
            
            // Create configuration panel with better positioning
            configPanel = new GameObject("ConfigPanel");
            configPanel.transform.SetParent(mainCanvas.transform, false);
            
            RectTransform panelRect = configPanel.AddComponent<RectTransform>();
            panelRect.anchorMin = new Vector2(0.3f, 0.2f);
            panelRect.anchorMax = new Vector2(0.7f, 0.8f);
            panelRect.sizeDelta = Vector2.zero;
            
            Image panelImage = configPanel.AddComponent<Image>();
            panelImage.color = new Color(0.1f, 0.1f, 0.2f, 0.95f);
            
            // Try to add rounded corners
            UIPrefabCreator prefabCreator = FindObjectOfType<UIPrefabCreator>();
            if (prefabCreator != null)
            {
                panelImage.sprite = prefabCreator.CreateRoundedRectSprite(15);
            }
            
            // Create title with better positioning
            GameObject titleObj = new GameObject("Title");
            titleObj.transform.SetParent(configPanel.transform, false);
            
            RectTransform titleRect = titleObj.AddComponent<RectTransform>();
            titleRect.anchorMin = new Vector2(0, 0.85f);
            titleRect.anchorMax = new Vector2(1, 0.95f);
            titleRect.sizeDelta = Vector2.zero;
            titleRect.anchoredPosition = Vector2.zero;
            
            TextMeshProUGUI titleText = titleObj.AddComponent<TextMeshProUGUI>();
            titleText.text = "BATTLE CONFIGURATION";
            titleText.fontSize = 36;
            titleText.color = Color.white;
            titleText.alignment = TextAlignmentOptions.Center;
            
            // Improve vertical spacing between elements
            playerPartySlider = CreateSlider("PlayerPartySlider", configPanel.transform, 0.7f, "Player Party Size: 3");
            enemyPartySlider = CreateSlider("EnemyPartySlider", configPanel.transform, 0.5f, "Enemy Party Size: 3");
            gameSpeedSlider = CreateSlider("GameSpeedSlider", configPanel.transform, 0.3f, "Game Speed: 1.0x");
            
            // Get text components - find labels in the panel instead of on sliders
            playerPartyText = configPanel.transform.Find("Label_PlayerPartySlider").GetComponent<TextMeshProUGUI>();
            enemyPartyText = configPanel.transform.Find("Label_EnemyPartySlider").GetComponent<TextMeshProUGUI>();
            gameSpeedText = configPanel.transform.Find("Label_GameSpeedSlider").GetComponent<TextMeshProUGUI>();
            
            // Setup slider values and listeners
                playerPartySlider.minValue = 1;
                playerPartySlider.maxValue = 5;
                playerPartySlider.value = playerPartySize;
                playerPartySlider.wholeNumbers = true;
                playerPartySlider.onValueChanged.AddListener(OnPlayerPartySizeChanged);
            
                enemyPartySlider.minValue = 1;
                enemyPartySlider.maxValue = 5;
                enemyPartySlider.value = enemyPartySize;
                enemyPartySlider.wholeNumbers = true;
                enemyPartySlider.onValueChanged.AddListener(OnEnemyPartySizeChanged);
            
                gameSpeedSlider.minValue = 0.1f;
                gameSpeedSlider.maxValue = 60.0f;
                gameSpeedSlider.value = gameSpeed;
                gameSpeedSlider.onValueChanged.AddListener(OnGameSpeedChanged);
            
            // Create start button with improved appearance
            GameObject startButtonObj = new GameObject("StartButton");
            startButtonObj.transform.SetParent(configPanel.transform, false);
            
            RectTransform startButtonRect = startButtonObj.AddComponent<RectTransform>();
            startButtonRect.anchorMin = new Vector2(0.3f, 0.05f);
            startButtonRect.anchorMax = new Vector2(0.7f, 0.15f);
            startButtonRect.sizeDelta = Vector2.zero;
            startButtonRect.anchoredPosition = Vector2.zero;
            
            Image buttonImage = startButtonObj.AddComponent<Image>();
            buttonImage.color = new Color(0.2f, 0.7f, 0.2f, 1f);
            buttonImage.raycastTarget = true;
            
            // Add rounded corners to button if possible
            if (prefabCreator != null)
            {
                buttonImage.sprite = prefabCreator.CreateRoundedRectSprite(10);
            }
            
            startBattleButton = startButtonObj.AddComponent<Button>();
            startBattleButton.targetGraphic = buttonImage;
            startBattleButton.onClick.AddListener(OnStartBattle);
            
            // Add a color transition to the button
            ColorBlock colors = startBattleButton.colors;
            colors.normalColor = new Color(0.2f, 0.7f, 0.2f, 1f);
            colors.highlightedColor = new Color(0.3f, 0.8f, 0.3f, 1f);
            colors.pressedColor = new Color(0.15f, 0.6f, 0.15f, 1f);
            startBattleButton.colors = colors;
            
            // CHANGE: Make button text more centered and clear
            GameObject buttonTextObj = new GameObject("Text");
            buttonTextObj.transform.SetParent(startButtonObj.transform, false);
            
            RectTransform buttonTextRect = buttonTextObj.AddComponent<RectTransform>();
            // Keep text centered inside button
            buttonTextRect.anchorMin = new Vector2(0, 0);
            buttonTextRect.anchorMax = new Vector2(1, 1);
            buttonTextRect.sizeDelta = Vector2.zero;
            buttonTextRect.anchoredPosition = Vector2.zero;
            
            TextMeshProUGUI buttonText = buttonTextObj.AddComponent<TextMeshProUGUI>();
            buttonText.text = "START BATTLE"; // Single line text
            buttonText.fontSize = 24; // Slightly smaller to fit better
            buttonText.fontStyle = FontStyles.Bold;
            buttonText.color = Color.white;
            buttonText.alignment = TextAlignmentOptions.Center; // Center alignment
            buttonText.verticalAlignment = VerticalAlignmentOptions.Middle; // Middle vertical alignment
            
            Debug.Log("Battle configuration UI created");
        }
        
        private Slider CreateSlider(string name, Transform parent, float yPosition, string labelText)
        {
            GameObject sliderObj = new GameObject(name);
            sliderObj.transform.SetParent(parent, false);
            
            // Position slider lower in its area
            RectTransform sliderRect = sliderObj.AddComponent<RectTransform>();
            sliderRect.anchorMin = new Vector2(0.1f, yPosition - 0.07f);  // Move slider down
            sliderRect.anchorMax = new Vector2(0.9f, yPosition - 0.02f);  // Shrink slider height
            sliderRect.sizeDelta = Vector2.zero;
            sliderRect.anchoredPosition = Vector2.zero;
            
            Slider slider = sliderObj.AddComponent<Slider>();
            
            // Create background
            GameObject bgObj = new GameObject("Background");
            bgObj.transform.SetParent(sliderObj.transform, false);
            
            RectTransform bgRect = bgObj.AddComponent<RectTransform>();
            bgRect.anchorMin = Vector2.zero;
            bgRect.anchorMax = Vector2.one;
            bgRect.sizeDelta = Vector2.zero;
            
            Image bgImage = bgObj.AddComponent<Image>();
            bgImage.color = new Color(0.2f, 0.2f, 0.2f, 1f);
            
            // Create fill area
            GameObject fillAreaObj = new GameObject("Fill Area");
            fillAreaObj.transform.SetParent(sliderObj.transform, false);
            
            RectTransform fillAreaRect = fillAreaObj.AddComponent<RectTransform>();
            fillAreaRect.anchorMin = new Vector2(0, 0);
            fillAreaRect.anchorMax = new Vector2(1, 1);
            fillAreaRect.sizeDelta = new Vector2(-20, -6);
            fillAreaRect.anchoredPosition = Vector2.zero;
            
            // Create fill
            GameObject fillObj = new GameObject("Fill");
            fillObj.transform.SetParent(fillAreaObj.transform, false);
            
            RectTransform fillRect = fillObj.AddComponent<RectTransform>();
            fillRect.anchorMin = Vector2.zero;
            fillRect.anchorMax = new Vector2(0.5f, 1);
            fillRect.sizeDelta = Vector2.zero;
            fillRect.anchoredPosition = Vector2.zero;
            
            Image fillImage = fillObj.AddComponent<Image>();
            fillImage.color = new Color(0.4f, 0.6f, 1f, 1f);
            
            // Create handle area
            GameObject handleAreaObj = new GameObject("Handle Slide Area");
            handleAreaObj.transform.SetParent(sliderObj.transform, false);
            
            RectTransform handleAreaRect = handleAreaObj.AddComponent<RectTransform>();
            handleAreaRect.anchorMin = Vector2.zero;
            handleAreaRect.anchorMax = Vector2.one;
            handleAreaRect.sizeDelta = Vector2.zero;
            handleAreaRect.anchoredPosition = Vector2.zero;
            
            // Create handle
            GameObject handleObj = new GameObject("Handle");
            handleObj.transform.SetParent(handleAreaObj.transform, false);
            
            RectTransform handleRect = handleObj.AddComponent<RectTransform>();
            handleRect.sizeDelta = new Vector2(20, 20);
            
            Image handleImage = handleObj.AddComponent<Image>();
            handleImage.color = Color.white;
            handleImage.raycastTarget = true;
            
            // Create completely separate label ABOVE the slider
            GameObject labelObj = new GameObject("Label");
            labelObj.transform.SetParent(parent, false); // Parent to panel instead of slider
            
            RectTransform labelRect = labelObj.AddComponent<RectTransform>();
            // Position label in a specific area above the slider
            labelRect.anchorMin = new Vector2(0.1f, yPosition + 0.01f);
            labelRect.anchorMax = new Vector2(0.9f, yPosition + 0.06f);
            labelRect.sizeDelta = Vector2.zero;
            labelRect.anchoredPosition = Vector2.zero;
            
            TextMeshProUGUI labelTextComponent = labelObj.AddComponent<TextMeshProUGUI>();
            labelTextComponent.text = labelText;
            labelTextComponent.fontSize = 24;
            labelTextComponent.color = Color.white;
            labelTextComponent.alignment = TextAlignmentOptions.Center;
            
            // Connect slider components
            slider.fillRect = fillRect;
            slider.handleRect = handleRect;
            slider.targetGraphic = handleImage;
            slider.direction = Slider.Direction.LeftToRight;
            slider.interactable = true;
            
            // Store a reference to the label with the slider
            labelObj.name = "Label_" + name;
            
            return slider;
        }
        
        private void OnPlayerPartySizeChanged(float value)
        {
            playerPartySize = (int)value;
            playerPartyText.text = $"Player Party Size: {playerPartySize}";
        }
        
        private void OnEnemyPartySizeChanged(float value)
        {
            enemyPartySize = (int)value;
            enemyPartyText.text = $"Enemy Party Size: {enemyPartySize}";
        }
        
        private void OnGameSpeedChanged(float value)
        {
            gameSpeed = value;
                gameSpeedText.text = $"Game Speed: {gameSpeed:F1}x";
        }
        
        private void OnStartBattle()
        {
            // Set configuration in controller
            if (battleSceneController != null)
            {
                Debug.Log($"Starting battle with: Players={playerPartySize}, Enemies={enemyPartySize}, Speed={gameSpeed}x");
                
                // Set party sizes
            battleSceneController.SetPartySize(playerPartySize, enemyPartySize);
            
                // Set game speed
                if (battleManager != null)
                {
            battleManager.SetBattleSpeed(gameSpeed);
                }
            
            // Hide config panel
                configPanel.SetActive(false);
            
            // Start the battle
            battleSceneController.StartBattle();
        }
            else
            {
                Debug.LogError("Battle scene controller is null!");
            }
        }
    }
} 