using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AstrobloxRPG
{
    /// <summary>
    /// Test script to verify battle system functionality
    /// </summary>
    public class BattleSystemTest : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private BattleSceneController battleSceneController;
        [SerializeField] private BattleManager battleManager;
        [SerializeField] private BattleUIManager uiManager;
        
        [Header("Test Configuration")]
        [SerializeField] private DefaultAgentConfigs defaultConfigs;
        [SerializeField] private bool autoStartTest = true;
        
        private void Start()
        {
            if (autoStartTest)
            {
                RunBattleTest();
            }
        }
        
        public void RunBattleTest()
        {
            Debug.Log("Starting battle system test...");
            
            // Verify components
            if (battleSceneController == null)
            {
                Debug.LogError("BattleSceneController is missing!");
                return;
            }
            
            if (battleManager == null)
            {
                Debug.LogError("BattleManager is missing!");
                return;
            }
            
            if (uiManager == null)
            {
                Debug.LogError("BattleUIManager is missing!");
                return;
            }
            
            if (defaultConfigs == null)
            {
                Debug.LogError("DefaultAgentConfigs is missing!");
                return;
            }
            
            // Configure player party with default configs
            List<AgentConfig> playerConfigs = new List<AgentConfig>
            {
                defaultConfigs.warriorConfig,
                defaultConfigs.healerConfig,
                defaultConfigs.mageConfig
            };
            
            // Configure enemy party with default configs
            List<AgentConfig> enemyConfigs = new List<AgentConfig>
            {
                defaultConfigs.goblinConfig,
                defaultConfigs.orcConfig,
                defaultConfigs.trollConfig
            };
            
            // Set configurations on battle scene controller
            SetTestConfigurations(playerConfigs, enemyConfigs);
            
            // Initialize battle
            Debug.Log("Battle test initialized successfully!");
        }
        
        private void SetTestConfigurations(List<AgentConfig> playerConfigs, List<AgentConfig> enemyConfigs)
        {
            // Use reflection to set private fields
            var controllerType = battleSceneController.GetType();
            
            // Set player configs
            var playerConfigsField = controllerType.GetField("playerConfigs", 
                System.Reflection.BindingFlags.Instance | 
                System.Reflection.BindingFlags.NonPublic | 
                System.Reflection.BindingFlags.Public);
            
            if (playerConfigsField != null)
            {
                playerConfigsField.SetValue(battleSceneController, playerConfigs);
            }
            
            // Set enemy configs
            var enemyConfigsField = controllerType.GetField("enemyConfigs", 
                System.Reflection.BindingFlags.Instance | 
                System.Reflection.BindingFlags.NonPublic | 
                System.Reflection.BindingFlags.Public);
            
            if (enemyConfigsField != null)
            {
                enemyConfigsField.SetValue(battleSceneController, enemyConfigs);
            }
        }
    }
}
