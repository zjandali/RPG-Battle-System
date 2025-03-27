using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AstrobloxRPG
{
    /// <summary>
    /// Sample agent configurations for testing
    /// </summary>
    [CreateAssetMenu(fileName = "DefaultAgentConfigs", menuName = "Astroblox RPG/Default Agent Configurations")]
    public class DefaultAgentConfigs : ScriptableObject
    {
        [Header("Player Configurations")]
        public AgentConfig warriorConfig;
        public AgentConfig healerConfig;
        public AgentConfig mageConfig;
        
        [Header("Enemy Configurations")]
        public AgentConfig goblinConfig;
        public AgentConfig orcConfig;
        public AgentConfig trollConfig;
        
        private void OnEnable()
        {
            // Initialize default configurations if they don't exist
            InitializeDefaultConfigs();
        }
        
        private void InitializeDefaultConfigs()
        {
            // Warrior config
            if (warriorConfig == null)
            {
                warriorConfig = CreateInstance<AgentConfig>();
                warriorConfig.name = "Warrior";
                warriorConfig.maxHealth = 150f;
                warriorConfig.attack = 25f;
                warriorConfig.defense = 15f;
                warriorConfig.speed = 10f;
                warriorConfig.availableActions = new List<ActionType>
                {
                    ActionType.Attack,
                    ActionType.BuffAlly,
                    ActionType.DebuffEnemy
                };
                warriorConfig.preferHealingAllies = false;
            }
            
            // Healer config
            if (healerConfig == null)
            {
                healerConfig = CreateInstance<AgentConfig>();
                healerConfig.name = "Healer";
                healerConfig.maxHealth = 100f;
                healerConfig.attack = 15f;
                healerConfig.defense = 10f;
                healerConfig.speed = 12f;
                healerConfig.availableActions = new List<ActionType>
                {
                    ActionType.Attack,
                    ActionType.Heal,
                    ActionType.HealOverTime,
                    ActionType.BuffAlly
                };
                healerConfig.preferHealingAllies = true;
            }
            
            // Mage config
            if (mageConfig == null)
            {
                mageConfig = CreateInstance<AgentConfig>();
                mageConfig.name = "Mage";
                mageConfig.maxHealth = 80f;
                mageConfig.attack = 30f;
                mageConfig.defense = 5f;
                mageConfig.speed = 8f;
                mageConfig.availableActions = new List<ActionType>
                {
                    ActionType.Attack,
                    ActionType.DamageOverTime,
                    ActionType.DebuffEnemy
                };
                mageConfig.preferHealingAllies = false;
            }
            
            // Goblin config
            if (goblinConfig == null)
            {
                goblinConfig = CreateInstance<AgentConfig>();
                goblinConfig.name = "Goblin";
                goblinConfig.maxHealth = 100f;
                goblinConfig.attack = 15f;
                goblinConfig.defense = 10f;
                goblinConfig.speed = 12f;
                goblinConfig.availableActions = new List<ActionType>
                {
                    ActionType.Attack,
                    ActionType.Heal,
                    ActionType.HealOverTime,
                    ActionType.BuffAlly
                };
            }
            
            // Orc config
            if (orcConfig == null)
            {
                orcConfig = CreateInstance<AgentConfig>();
                orcConfig.name = "Orc";
                orcConfig.maxHealth = 150f;
                orcConfig.attack = 25f;
                orcConfig.defense = 15f;
                orcConfig.speed = 10f;
                orcConfig.availableActions = new List<ActionType>
                {
                    ActionType.Attack,
                    ActionType.BuffAlly,
                    ActionType.DebuffEnemy
                };
            }
            
            // Troll config
            if (trollConfig == null)
            {
                trollConfig = CreateInstance<AgentConfig>();
                trollConfig.name = "Troll";
                trollConfig.maxHealth = 80f;
                trollConfig.attack = 30f;
                trollConfig.defense = 5f;
                trollConfig.speed = 8f;
                trollConfig.availableActions = new List<ActionType>
                {
                    ActionType.Attack,
                    ActionType.DamageOverTime,
                    ActionType.DebuffEnemy
                };
            }
        }

        public void CreateDefaultConfigs()
        {
            // WARRIOR & ORC - Make identical
            float warriorHealth = 150f;
            float warriorAttack = 25f;
            float warriorDefense = 15f;
            float warriorSpeed = 10f;
            
            warriorConfig.maxHealth = warriorHealth;
            warriorConfig.attack = warriorAttack;
            warriorConfig.defense = warriorDefense;
            warriorConfig.speed = warriorSpeed;
            warriorConfig.availableActions = new List<ActionType>
            {
                ActionType.Attack,
                ActionType.BuffAlly,
                ActionType.DebuffEnemy
            };
            warriorConfig.preferHealingAllies = false;
            
            orcConfig.maxHealth = warriorHealth;
            orcConfig.attack = warriorAttack;
            orcConfig.defense = warriorDefense; 
            orcConfig.speed = warriorSpeed;
            orcConfig.availableActions = new List<ActionType>
            {
                ActionType.Attack,
                ActionType.BuffAlly,
                ActionType.DebuffEnemy
            };
            orcConfig.preferHealingAllies = false; // Make behavior identical
            
            // HEALER & GOBLIN - Make identical
            float healerHealth = 100f;
            float healerAttack = 15f;
            float healerDefense = 10f;
            float healerSpeed = 12f;
            
            healerConfig.maxHealth = healerHealth;
            healerConfig.attack = healerAttack;
            healerConfig.defense = healerDefense;
            healerConfig.speed = healerSpeed;
            healerConfig.availableActions = new List<ActionType>
            {
                ActionType.Attack,
                ActionType.Heal,
                ActionType.HealOverTime,
                ActionType.BuffAlly
            };
            healerConfig.preferHealingAllies = true;
            
            goblinConfig.maxHealth = healerHealth;
            goblinConfig.attack = healerAttack;
            goblinConfig.defense = healerDefense;
            goblinConfig.speed = healerSpeed;
            goblinConfig.availableActions = new List<ActionType>
            {
                ActionType.Attack,
                ActionType.Heal,
                ActionType.HealOverTime,
                ActionType.BuffAlly
            };
            goblinConfig.preferHealingAllies = true; // Make behavior identical
            
            // MAGE & TROLL - Make identical
            float mageHealth = 80f;
            float mageAttack = 30f;
            float mageDefense = 5f;
            float mageSpeed = 8f;
            
            mageConfig.maxHealth = mageHealth;
            mageConfig.attack = mageAttack;
            mageConfig.defense = mageDefense;
            mageConfig.speed = mageSpeed;
            mageConfig.availableActions = new List<ActionType>
            {
                ActionType.Attack,
                ActionType.DamageOverTime,
                ActionType.DebuffEnemy
            };
            
            trollConfig.maxHealth = mageHealth;
            trollConfig.attack = mageAttack;
            trollConfig.defense = mageDefense;
            trollConfig.speed = mageSpeed;
            trollConfig.availableActions = new List<ActionType>
            {
                ActionType.Attack,
                ActionType.DamageOverTime,
                ActionType.DebuffEnemy
            };
        }
    }
}
