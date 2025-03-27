using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AstrobloxRPG
{
    /// <summary>
    /// Main battle manager that controls the flow of battle
    /// </summary>
    public class BattleManager : MonoBehaviour
    {
        [Header("Battle Configuration")]
        [SerializeField] private Party playerParty;
        [SerializeField] private Party enemyParty;
        
        // Getters for UI manager
        public Party GetPlayerParty() => playerParty;
        public Party GetEnemyParty() => enemyParty;
        
        [Header("Battle Settings")]
        
        // Battle state
        private bool isBattleActive = false;
        private bool isBattleOver = false;
        
        // Events
        public delegate void BattleEvent();
        public event BattleEvent OnBattleStart;
        public event BattleEvent OnBattleEnd;
        public event BattleEvent OnPlayerVictory;
        public event BattleEvent OnPlayerDefeat;
        
        [SerializeField] private float battleSpeed = 1.0f;
        
        public void StartBattle()
        {
            // Initialize both parties
            playerParty.Initialize(this);
            enemyParty.Initialize(this);
            
            // Subscribe to party defeat events
            playerParty.OnPartyDefeated += HandlePlayerPartyDefeated;
            enemyParty.OnPartyDefeated += HandleEnemyPartyDefeated;
            
            // Start the battle
            isBattleActive = true;
            isBattleOver = false;
            
            // Trigger battle start event
            OnBattleStart?.Invoke();
            
            Debug.Log("Battle started!");
        }
        
        private void Update()
        {
            if (!isBattleActive || isBattleOver) return;
            
            // Apply battleSpeed to deltaTime
            float adjustedDeltaTime = Time.deltaTime * battleSpeed;
            
            // Update both parties with the adjusted time
            playerParty.UpdateParty(adjustedDeltaTime);
            enemyParty.UpdateParty(adjustedDeltaTime);
            
            // Check for battle end conditions
            CheckBattleEndConditions();
        }
        
        private void CheckBattleEndConditions()
        {
            // Check if either party has no alive agents
            if (!playerParty.HasAliveAgents() || !enemyParty.HasAliveAgents())
            {
                // Log the final health stats for analysis
                Debug.Log("===== BATTLE END STATISTICS =====");
                
                Debug.Log("PLAYER PARTY:");
                foreach (Agent agent in playerParty.Agents)
                {
                    Debug.Log($"{agent.gameObject.name}: {agent.GetCurrentHealth()}/{agent.GetMaxHealth()} HP, ATK: {agent.GetAttack()}, DEF: {agent.GetDefense()}, SPD: {agent.GetSpeed()}");
                }
                
                Debug.Log("ENEMY PARTY:");
                foreach (Agent agent in enemyParty.Agents)
                {
                    Debug.Log($"{agent.gameObject.name}: {agent.GetCurrentHealth()}/{agent.GetMaxHealth()} HP, ATK: {agent.GetAttack()}, DEF: {agent.GetDefense()}, SPD: {agent.GetSpeed()}");
                }
                
                Debug.Log("===============================");
                
                EndBattle();
            }
        }
        
        private void HandlePlayerPartyDefeated(Party party)
        {
            if (isBattleOver) return;
            
            Debug.Log("Player party defeated!");
            OnPlayerDefeat?.Invoke();
            EndBattle();
        }
        
        private void HandleEnemyPartyDefeated(Party party)
        {
            if (isBattleOver) return;
            
            Debug.Log("Enemy party defeated!");
            OnPlayerVictory?.Invoke();
            EndBattle();
        }
        
        private void EndBattle()
        {
            isBattleActive = false;
            isBattleOver = true;
            
            // Trigger battle end event
            OnBattleEnd?.Invoke();
            
            Debug.Log("Battle ended!");
        }
        
        // Helper methods for agents to find targets
        
        public Player GetRandomPlayer()
        {
            Agent agent = playerParty.GetRandomAgent();
            return agent as Player;
        }
        
        public Enemy GetRandomEnemy()
        {
            Agent agent = enemyParty.GetRandomAgent();
            return agent as Enemy;
        }
        
        public Player GetLowestHealthAlly(Player player)
        {
            Agent agent = playerParty.GetLowestHealthAgent();
            if (agent == player) return null; // Don't return self
            return agent as Player;
        }
        
        public Enemy GetLowestHealthEnemyAlly(Enemy enemy)
        {
            Agent agent = enemyParty.GetLowestHealthAgent();
            if (agent == enemy) return null; // Don't return self
            return agent as Enemy;
        }
        
        public Player GetRandomAlly(Player player)
        {
            // Get a list of all players except the current one
            List<Agent> allies = new List<Agent>();
            foreach (Agent agent in playerParty.Agents)
            {
                if (agent != player && agent.IsAlive)
                {
                    allies.Add(agent);
                }
            }
            
            if (allies.Count == 0) return null;
            
            int randomIndex = Random.Range(0, allies.Count);
            return allies[randomIndex] as Player;
        }
        
        public Enemy GetRandomEnemyAlly(Enemy enemy)
        {
            // Get a list of all enemies except the current one
            List<Agent> allies = new List<Agent>();
            foreach (Agent agent in enemyParty.Agents)
            {
                if (agent != enemy && agent.IsAlive)
                {
                    allies.Add(agent);
                }
            }
            
            if (allies.Count == 0) return null;
            
            int randomIndex = Random.Range(0, allies.Count);
            return allies[randomIndex] as Enemy;
        }

        public void EndBattle(bool playerVictory)
        {
            // Log to verify this is being called
            Debug.Log("Battle ended with player victory: " + playerVictory);
            
            // Trigger the battle end event
            OnBattleEnd?.Invoke();
            
            // Trigger the appropriate victory/defeat event
            if (playerVictory)
            {
                Debug.Log("Triggering player victory event");
                OnPlayerVictory?.Invoke();
            }
            else
            {
                Debug.Log("Triggering player defeat event");
                OnPlayerDefeat?.Invoke();
            }
        }

        public void SetBattleSpeed(float speed)
        {
            battleSpeed = speed;
            Debug.Log($"Battle speed set to {battleSpeed}x");
            
            // Find the battle timer and update its speed if it exists
            BattleTimer timer = FindObjectOfType<BattleTimer>();
            if (timer != null)
            {
                timer.SetTimeScale(battleSpeed);
            }
        }

        public float GetBattleSpeed()
        {
            return battleSpeed;
        }
    }
}
