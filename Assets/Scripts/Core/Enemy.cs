using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace AstrobloxRPG
{
    /// <summary>
    /// Enemy agent class that fights against players
    /// </summary>
    public class Enemy : Agent
    {
        [Header("Enemy Settings")]
        [SerializeField] private List<ActionType> availableActions = new List<ActionType>();
        
        protected override void TakeAction()
        {
            if (battleManager == null) return;
            
            // Choose an action based on available actions
            ActionType actionType = ChooseActionType();
            
            // Use the same switch pattern as Player class
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
        
        // Renamed to avoid conflict with the inherited virtual method
        private ActionType ChooseActionType()
        {
            // If no available actions, default to attack
            if (availableActions.Count == 0)
            {
                return ActionType.Attack;
            }
            
            // Get all agents to analyze the battlefield situation
            List<Agent> allies = new List<Agent>(battleManager.GetEnemyParty().Agents);
            List<Agent> enemies = new List<Agent>(battleManager.GetPlayerParty().Agents);
            
            // Check for healing needs - prioritize healing if any ally is below 50% health
            if (availableActions.Contains(ActionType.Heal))
            {
                Agent lowestHealthAlly = battleManager.GetLowestHealthEnemyAlly(this);
                if (lowestHealthAlly != null && lowestHealthAlly.GetCurrentHealth() / lowestHealthAlly.GetMaxHealth() < 0.5f)
                {
                    // 80% chance to heal when ally is below 50% health, same as player logic
                    if (Random.value < 0.8f)
                    {
                        return ActionType.Heal;
                    }
                }
            }
            
            // Check for buff/debuff opportunities
            bool anyLowHealth = allies.Any(a => a.GetCurrentHealth() / a.GetMaxHealth() < 0.3f);
            
            // If team is in danger (has low health members), prioritize debuffs
            if (anyLowHealth && availableActions.Contains(ActionType.DebuffEnemy) && Random.value < 0.7f)
            {
                return ActionType.DebuffEnemy;
            }
            
            // Select a completely random action with equal probabilities
            return availableActions[Random.Range(0, availableActions.Count)];
        }
        
        // Change this method to be private without override keyword
        private ActionType ChooseAction()
        {
            // Your method implementation here
            // This should have similar logic to your ChooseActionType() method
            return ActionType.Attack; // Placeholder return, actual implementation needed
        }
        
        private void PerformAttack()
        {
            // Find a player target
            Player target = battleManager.GetRandomPlayer();
            if (target == null) return;
            
            // Calculate raw damage
            float rawDamage = currentAttack;
            
            // Log the attack intention
            Debug.Log($"[ATTACK] {gameObject.name} attacks {target.gameObject.name} with {rawDamage} raw damage!");
            
            // Pass this (the source) to TakeDamage
            target.TakeDamage(rawDamage, this);
        }
        
        private void PerformHeal()
        {
            // Find the ally with lowest health
            Enemy target = battleManager.GetLowestHealthEnemyAlly(this);
            
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
            
            // Heal based on a percentage of attack stat
            float healAmount = currentAttack * 0.8f;
            target.Heal(healAmount);
            
            // Log the action
            Debug.Log($"{gameObject.name} heals {target.gameObject.name} for {healAmount} health!");
        }
        
        private void PerformBuffAlly()
        {
            // Find a random ally
            Enemy target = battleManager.GetRandomEnemyAlly(this);
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
            // Find a player target
            Player target = battleManager.GetRandomPlayer();
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
            // Find a player target
            Player target = battleManager.GetRandomPlayer();
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
            Enemy target = battleManager.GetLowestHealthEnemyAlly(this);
            
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
        private Enemy FindAnyAllyNeedingHealing()
        {
            if (battleManager == null) return null;
            
            List<Agent> allies = new List<Agent>();
            foreach (Agent agent in battleManager.GetEnemyParty().Agents)
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
            
            return allies[0] as Enemy;
        }

        // Helper method to perform an alternative action
        private void PerformAlternativeAction(string originalAction)
        {
            Debug.Log($"{gameObject.name} would have used {originalAction} but all allies are at full health. Choosing another action...");
            
            // Try to debuff a player if available
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
}
