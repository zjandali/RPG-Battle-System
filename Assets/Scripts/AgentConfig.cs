using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AstrobloxRPG
{
    /// <summary>
    /// ScriptableObject for configuring agent stats and actions
    /// </summary>
    [CreateAssetMenu(fileName = "New Agent Config", menuName = "Astroblox RPG/Agent Configuration")]
    public class AgentConfig : ScriptableObject
    {
        [Header("Base Stats")]
        public float maxHealth = 100f;
        public float attack = 20f;
        public float defense = 10f;
        public float speed = 10f;
        
        [Header("Available Actions")]
        public List<ActionType> availableActions = new List<ActionType>();
        
        [Header("Behavior Settings")]
        public bool preferHealingAllies = true; // Only used for players
        
        /// <summary>
        /// Apply this configuration to an agent
        /// </summary>
        public void ApplyToAgent(Agent agent)
        {
            // Use reflection to set private fields
            var agentType = agent.GetType();
            
            // Set base stats
            SetPrivateField(agent, "maxHealth", maxHealth);
            SetPrivateField(agent, "attack", attack);
            SetPrivateField(agent, "defense", defense);
            SetPrivateField(agent, "speed", speed);
            
            // Set available actions if it's a player
            if (agent is Player player)
            {
                SetPrivateField(player, "availableActions", new List<ActionType>(availableActions));
                SetPrivateField(player, "preferHealingAllies", preferHealingAllies);
            }
            
            // Set available actions if it's an enemy
            if (agent is Enemy enemy)
            {
                SetPrivateField(enemy, "availableActions", new List<ActionType>(availableActions));
            }
            
            // Reset the agent's stats
            agent.Initialize(null); // BattleManager will be set later
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
    }
}
