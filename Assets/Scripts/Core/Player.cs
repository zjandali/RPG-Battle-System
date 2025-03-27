using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace AstrobloxRPG
{
    /// <summary>
    /// Player agent class that fights against enemies
    /// </summary>
    public class Player : Agent
    {
        [Header("Player Settings")]
        [SerializeField] private List<ActionType> availableActions = new List<ActionType>();
        [SerializeField] private bool preferHealingAllies = true;
        
        protected override void TakeAction()
        {
            if (battleManager == null) return;
            
            // Choose an action based on available actions
            ActionType actionType = ChooseAction();
            
            switch (actionType)
            {
                case ActionType.Attack:
                    PerformAttack();
                    break;
                case ActionType.Heal:
                    PerformHeal();
                    break;
                case ActionType.BuffAlly:
                    PerformBuffAlly();
                    break;
                case ActionType.DebuffEnemy:
                    PerformDebuffEnemy();
                    break;
                case ActionType.DamageOverTime:
                    PerformDamageOverTime();
                    break;
                case ActionType.HealOverTime:
                    PerformHealOverTime();
                    break;
            }
        }
        
        private ActionType ChooseAction()
        {
            // If no actions available, default to attack
            if (availableActions.Count == 0)
                return ActionType.Attack;
            
            // Get all agents to analyze the battlefield situation
            List<Agent> allies = new List<Agent>(battleManager.GetPlayerParty().Agents);
            List<Agent> enemies = new List<Agent>(battleManager.GetEnemyParty().Agents);
            
            // Check if any ally needs healing and we can heal
            if (availableActions.Contains(ActionType.Heal))
            {
                Agent lowestHealthAlly = battleManager.GetLowestHealthAlly(this);
                if (lowestHealthAlly != null && lowestHealthAlly.GetCurrentHealth() / lowestHealthAlly.GetMaxHealth() < 0.5f)
                {
                    // Exactly 80% chance to heal when ally is below 50% health - same as enemy
                    if (Random.value < 0.8f)
                    {
                        return ActionType.Heal;
                    }
                }
            }
            
            // Check for buff/debuff opportunities - using exact same logic as enemy
            bool anyLowHealth = allies.Any(a => a.GetCurrentHealth() / a.GetMaxHealth() < 0.3f);
            
            // If team is in danger (has low health members), prioritize debuffs - same as enemy logic
            if (anyLowHealth && availableActions.Contains(ActionType.DebuffEnemy) && Random.value < 0.7f)
            {
                return ActionType.DebuffEnemy;
            }
            
            // Select a completely random action with equal probabilities - exactly as in enemy logic
            return availableActions[Random.Range(0, availableActions.Count)];
        }
        
        private void PerformAttack()
        {
            // Find an enemy target
            Enemy target = battleManager.GetRandomEnemy();
            if (target == null) return;
            
            // Calculate raw damage - exactly the same as enemy's calculation
            float rawDamage = currentAttack;
            
            // Log the attack intention - same format as enemy
            Debug.Log($"[ATTACK] {gameObject.name} attacks {target.gameObject.name} with {rawDamage} raw damage!");
            
            // Pass this (the source) to TakeDamage - same as enemy
            target.TakeDamage(rawDamage, this);
        }
        
        private void PerformHeal()
        {
            // Find the ally with lowest health
            Player target = battleManager.GetLowestHealthAlly(this);
            
            // Check if target exists and needs healing
            if (target == null || Mathf.Approximately(target.GetCurrentHealth(), target.GetMaxHealth()))
            {
                // Try to find ANY ally that needs healing
                target = FindAnyAllyNeedingHealing();
                
                // If no allies need healing, do something else instead
                if (target == null)
                {
                    // Choose another action instead
                    PerformAlternativeAction("Heal");
                    return;
                }
            }
            
            // Heal based on a percentage of attack stat - exactly same as enemy
            float healAmount = currentAttack * 0.8f;
            target.Heal(healAmount);
            
            // Log the action
            Debug.Log($"{gameObject.name} heals {target.gameObject.name} for {healAmount} health!");
        }
        
        private void PerformBuffAlly()
        {
            // Find a random ally
            Player target = battleManager.GetRandomAlly(this);
            if (target == null) return;
            
            // Choose a random stat to buff
            StatType statType = (StatType)Random.Range(0, 3);
            float buffAmount = 5f + currentAttack * 0.1f;
            float duration = 10f;
            
            target.ApplyBuff(statType, buffAmount, duration);
            
            // Log the action
            Debug.Log($"{gameObject.name} buffs {target.gameObject.name}'s {statType} by {buffAmount} for {duration} seconds!");
        }
        
        private void PerformDebuffEnemy()
        {
            // Find an enemy target
            Enemy target = battleManager.GetRandomEnemy();
            if (target == null) return;
            
            // Choose a random stat to debuff
            StatType statType = (StatType)Random.Range(0, 3);
            float debuffAmount = 5f + currentAttack * 0.1f;
            float duration = 8f;
            
            target.ApplyDebuff(statType, debuffAmount, duration);
            
            // Log the action
            Debug.Log($"{gameObject.name} debuffs {target.gameObject.name}'s {statType} by {debuffAmount} for {duration} seconds!");
        }
        
        private void PerformDamageOverTime()
        {
            // Find an enemy target
            Enemy target = battleManager.GetRandomEnemy();
            if (target == null) return;
            
            // Apply damage over time
            float damagePerTick = currentAttack * 0.2f;
            float tickInterval = 2f;
            float duration = 8f;
            
            target.ApplyDamageOverTime(damagePerTick, tickInterval, duration);
            
            // Log the action
            Debug.Log($"{gameObject.name} applies DoT to {target.gameObject.name} for {damagePerTick} damage every {tickInterval} seconds for {duration} seconds!");
        }
        
        private void PerformHealOverTime()
        {
            // Find the ally with lowest health
            Player target = battleManager.GetLowestHealthAlly(this);
            
            // Check if target exists and needs healing
            if (target == null || Mathf.Approximately(target.GetCurrentHealth(), target.GetMaxHealth()))
            {
                // Try to find ANY ally that needs healing
                target = FindAnyAllyNeedingHealing();
                
                // If no allies need healing, do something else instead
                if (target == null)
                {
                    // Choose another action instead
                    PerformAlternativeAction("HealOverTime");
                    return;
                }
            }
            
            // Apply heal over time
            float healPerTick = currentAttack * 0.15f;
            float tickInterval = 2f;
            float duration = 10f;
            
            target.ApplyHealOverTime(healPerTick, tickInterval, duration);
            
            // Log the action
            Debug.Log($"{gameObject.name} applies HoT to {target.gameObject.name} for {healPerTick} healing every {tickInterval} seconds for {duration} seconds!");
        }

        // Helper method to find any ally that needs healing
        private Player FindAnyAllyNeedingHealing()
        {
            if (battleManager == null) return null;
            
            List<Agent> allies = new List<Agent>();
            foreach (Agent agent in battleManager.GetPlayerParty().Agents)
            {
                // Skip self and check if ally needs healing
                if (agent != this && agent.IsAlive && agent.GetCurrentHealth() < agent.GetMaxHealth())
                {
                    allies.Add(agent);
                }
            }
            
            if (allies.Count == 0) return null;
            
            // Sort by health percentage (lowest first)
            allies.Sort((a, b) => (a.GetCurrentHealth() / a.GetMaxHealth()).CompareTo(b.GetCurrentHealth() / b.GetMaxHealth()));
            
            return allies[0] as Player;
        }

        // Helper method to perform an alternative action
        private void PerformAlternativeAction(string originalAction)
        {
            Debug.Log($"{gameObject.name} would have used {originalAction} but all allies are at full health. Choosing another action...");
            
            // Try to debuff an enemy if available
            if (availableActions.Contains(ActionType.DebuffEnemy))
            {
                PerformDebuffEnemy();
            }
            // Try to buff an ally if available
            else if (availableActions.Contains(ActionType.BuffAlly))
            {
                PerformBuffAlly();
            }
            // Try damage over time if available
            else if (availableActions.Contains(ActionType.DamageOverTime))
            {
                PerformDamageOverTime();
            }
            // Fallback to basic attack
            else
            {
                PerformAttack();
            }
        }
    }
    
    /// <summary>
    /// Available action types for agents
    /// </summary>
    public enum ActionType
    {
        Attack,
        Heal,
        BuffAlly,
        DebuffEnemy,
        DamageOverTime,
        HealOverTime
    }
}
