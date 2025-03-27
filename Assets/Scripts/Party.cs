using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AstrobloxRPG
{
    /// <summary>
    /// Manages a party of agents (either players or enemies)
    /// </summary>
    public class Party : MonoBehaviour
    {
        [Header("Party Configuration")]
        [SerializeField] private string partyName;
        [SerializeField] private bool isPlayerParty = true;
        [SerializeField] private List<AgentConfig> agentConfigs = new List<AgentConfig>();
        
        public void SetAgentConfigs(List<AgentConfig> configs)
        {
            agentConfigs = new List<AgentConfig>(configs);
        }
        
        // List of active agents in this party
        private List<Agent> agents = new List<Agent>();
        
        // Reference to the battle manager
        private BattleManager battleManager;
        
        // Event for party defeat
        public delegate void PartyEvent(Party party);
        public event PartyEvent OnPartyDefeated;
        
        public string PartyName => partyName;
        public bool IsPlayerParty => isPlayerParty;
        public List<Agent> Agents => agents;
        
        public void Initialize(BattleManager manager)
        {
            battleManager = manager;
            
            // Clear any existing agents
            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }
            agents.Clear();
            
            // Create agents based on configurations
            for (int i = 0; i < agentConfigs.Count; i++)
            {
                CreateAgent(agentConfigs[i], i);
            }
        }
        
        private void CreateAgent(AgentConfig config, int index)
        {
            // Create a new game object for the agent
            GameObject agentObject = new GameObject($"{(isPlayerParty ? "Player" : "Enemy")}_{index + 1}");
            agentObject.transform.SetParent(transform);
            
            // Add the appropriate component based on party type
            Agent agent;
            if (isPlayerParty)
            {
                agent = agentObject.AddComponent<Player>();
            }
            else
            {
                agent = agentObject.AddComponent<Enemy>();
            }
            
            // Apply the configuration
            config.ApplyToAgent(agent);
            
            // Initialize the agent with the battle manager
            agent.Initialize(battleManager);
            
            // Subscribe to agent death event
            agent.OnDeath += HandleAgentDeath;
            
            // Add to the list of active agents
            agents.Add(agent);
        }
        
        private void HandleAgentDeath(Agent agent)
        {
            // Check if all agents are defeated
            bool allDefeated = true;
            foreach (Agent a in agents)
            {
                if (a.IsAlive)
                {
                    allDefeated = false;
                    break;
                }
            }
            
            if (allDefeated)
            {
                OnPartyDefeated?.Invoke(this);
            }
        }
        
        public void UpdateParty(float deltaTime)
        {
            // Update all agents in the party
            foreach (Agent agent in agents)
            {
                agent.UpdateAgent(deltaTime);
            }
        }
        
        public Agent GetRandomAgent()
        {
            List<Agent> aliveAgents = GetAliveAgents();
            if (aliveAgents.Count == 0) return null;
            
            int randomIndex = Random.Range(0, aliveAgents.Count);
            return aliveAgents[randomIndex];
        }
        
        public Agent GetLowestHealthAgent()
        {
            List<Agent> aliveAgents = GetAliveAgents();
            if (aliveAgents.Count == 0) return null;
            
            Agent lowestHealthAgent = aliveAgents[0];
            float lowestHealthPercentage = lowestHealthAgent.GetCurrentHealth() / lowestHealthAgent.GetMaxHealth();
            
            for (int i = 1; i < aliveAgents.Count; i++)
            {
                float healthPercentage = aliveAgents[i].GetCurrentHealth() / aliveAgents[i].GetMaxHealth();
                if (healthPercentage < lowestHealthPercentage)
                {
                    lowestHealthPercentage = healthPercentage;
                    lowestHealthAgent = aliveAgents[i];
                }
            }
            
            return lowestHealthAgent;
        }
        
        private List<Agent> GetAliveAgents()
        {
            List<Agent> aliveAgents = new List<Agent>();
            foreach (Agent agent in agents)
            {
                if (agent.IsAlive)
                {
                    aliveAgents.Add(agent);
                }
            }
            return aliveAgents;
        }
        
        public bool HasAliveAgents()
        {
            return GetAliveAgents().Count > 0;
        }
    }
}
