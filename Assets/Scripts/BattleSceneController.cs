using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AstrobloxRPG
{
    /// <summary>
    /// Main battle scene controller
    /// </summary>
    public class BattleSceneController : MonoBehaviour
    {
        [Header("Battle Configuration")]
        [SerializeField] private BattleManager battleManager;
        [SerializeField] private BattleUIManager uiManager;
        
        [Header("Player Party Configuration")]
        [SerializeField] private int playerPartySize = 3;
        [SerializeField] private List<AgentConfig> playerConfigs = new List<AgentConfig>();
        
        [Header("Enemy Party Configuration")]
        [SerializeField] private int enemyPartySize = 3;
        [SerializeField] private List<AgentConfig> enemyConfigs = new List<AgentConfig>();
        
        private void Start()
        {
            // Find or create battle config manager
            BattleConfigManager configManager = FindObjectOfType<BattleConfigManager>();
            if (configManager == null)
            {
                GameObject configObj = new GameObject("BattleConfigManager");
                configManager = configObj.AddComponent<BattleConfigManager>();
            }
            
            // Initialize the config manager
            configManager.Initialize(this, battleManager);
            
            // Don't automatically start the battle - wait for config
            // InitializeBattleScene(); - Remove this
        }
        
        private void InitializeBattleScene()
        {
            // Configure player party
            ConfigureParty(battleManager.GetPlayerParty(), playerConfigs, playerPartySize);
            
            // Configure enemy party
            ConfigureParty(battleManager.GetEnemyParty(), enemyConfigs, enemyPartySize);
            
            // Initialize UI
            if (uiManager != null)
            {
                uiManager.Initialize(battleManager);
            }
            
            // Start the battle
            battleManager.StartBattle();
        }
        
        private void ConfigureParty(Party party, List<AgentConfig> configs, int partySize)
        {
            // Create a list of agent configs to use
            List<AgentConfig> configsToUse = new List<AgentConfig>();
            
            // If we have enough configs, use them
            if (configs.Count >= partySize)
            {
                for (int i = 0; i < partySize; i++)
                {
                    configsToUse.Add(configs[i]);
                }
            }
            // Otherwise, repeat configs as needed
            else
            {
                for (int i = 0; i < partySize; i++)
                {
                    configsToUse.Add(configs[i % configs.Count]);
                }
            }
            
            // Set the configs on the party
            party.SetAgentConfigs(configsToUse);
        }

        public void SetPartySize(int playerSize, int enemySize)
        {
            playerPartySize = playerSize;
            enemyPartySize = enemySize;
            Debug.Log($"Party sizes set to: Players={playerPartySize}, Enemies={enemyPartySize}");
        }

        public void StartBattle()
        {
            Debug.Log("Starting battle from configuration!");
            InitializeBattleScene();
        }
    }
}
