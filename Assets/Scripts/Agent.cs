using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AstrobloxRPG
{
    /// <summary>
    /// Base class for all battle agents (players and enemies)
    /// </summary>
    public abstract class Agent : MonoBehaviour
    {
        [Header("Base Stats")]
        [SerializeField] protected float maxHealth;
        [SerializeField] protected float attack;
        [SerializeField] protected float defense;
        [SerializeField] protected float speed;

        // Current stats that can be modified by buffs/debuffs
        protected float currentHealth;
        protected float currentAttack;
        protected float currentDefense;
        protected float currentSpeed;

        // Action cooldown timer
        protected float actionTimer = 0f;

        // List of active effects on this agent
        protected List<Effect> activeEffects = new List<Effect>();

        // Reference to the battle manager
        protected BattleManager battleManager;

        // Flag to check if agent is alive
        public bool IsAlive { get; protected set; } = true;

        // Property to get action readiness percentage (0-1)
        public float ActionReadiness => actionTimer / (100f / currentSpeed);

        // Events
        public delegate void AgentEvent(Agent agent);
        public event AgentEvent OnDeath;
        public event AgentEvent OnDamaged;
        public event AgentEvent OnHealed;
        public event AgentEvent OnBuffed;
        public event AgentEvent OnDebuffed;

        protected virtual void Awake()
        {
            // Initialize current stats to base stats
            ResetStats();
        }

        public virtual void Initialize(BattleManager manager)
        {
            battleManager = manager;
            ResetStats();
            IsAlive = true;
            actionTimer = 0f;
            activeEffects.Clear();
        }

        protected void ResetStats()
        {
            currentHealth = maxHealth;
            currentAttack = attack;
            currentDefense = defense;
            currentSpeed = speed;
        }

        public virtual void UpdateAgent(float deltaTime)
        {
            if (!IsAlive) return;
            
            // Update active effects
            UpdateEffects(deltaTime);
            
            // Update action timer
            actionTimer += deltaTime * currentSpeed;
            
            // If action bar is full, take action
            if (actionTimer >= 100f)
            {
                TakeAction();
                
                // Reset action timer
                actionTimer = 0f;
                
                // Log whenever an action is taken to help debug
                Debug.Log($"{gameObject.name} took an action. Speed: {currentSpeed}");
            }
        }

        public void UpdateEffects(float deltaTime)
        {
            // Create a copy of the activeEffects list to safely iterate through
            List<Effect> effectsCopy = new List<Effect>(activeEffects);
            
            // First pass: update all effects
            foreach (Effect effect in effectsCopy)
            {
                effect.UpdateEffect(deltaTime);
            }
            
            // Second pass: remove inactive effects
            // Use RemoveAll method which is safe for bulk operations
            activeEffects.RemoveAll(effect => !effect.IsActive);
        }

        protected abstract void TakeAction();

        public virtual void TakeDamage(float amount, Agent source = null)
        {
            if (!IsAlive) return;
            
            // Calculate damage reduction based on defense
            float damageReduction = currentDefense / (currentDefense + 100f);
            float mitigatedAmount = amount * damageReduction;
            float actualDamage = amount - mitigatedAmount;
            
            // Apply the damage
            currentHealth -= actualDamage;
            
            // Log detailed damage information for debugging
            Debug.Log($"{gameObject.name} takes {actualDamage:F1} damage ({mitigatedAmount:F1} mitigated by defense). Health: {currentHealth:F1}/{maxHealth:F1}");
            
            // Check if agent is dead
            if (currentHealth <= 0)
            {
                currentHealth = 0;
                Die();
            }
            
            // Notify UI of damage
            OnDamaged?.Invoke(this);
        }

        public virtual void Heal(float amount)
        {
            currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
            OnHealed?.Invoke(this);
        }

        public virtual void ApplyBuff(StatType statType, float amount, float duration)
        {
            BuffEffect buff = new BuffEffect(this, statType, amount, duration);
            activeEffects.Add(buff);
            
            // Apply the buff immediately
            ApplyStatModifier(statType, amount);
            
            OnBuffed?.Invoke(this);
        }

        public virtual void ApplyDebuff(StatType statType, float amount, float duration)
        {
            DebuffEffect debuff = new DebuffEffect(this, statType, amount, duration);
            activeEffects.Add(debuff);
            
            // Apply the debuff immediately
            ApplyStatModifier(statType, -amount);
            
            OnDebuffed?.Invoke(this);
        }

        public virtual void ApplyDamageOverTime(float damagePerTick, float tickInterval, float duration)
        {
            DamageOverTimeEffect dot = new DamageOverTimeEffect(this, damagePerTick, tickInterval, duration);
            activeEffects.Add(dot);
        }

        public virtual void ApplyHealOverTime(float healPerTick, float tickInterval, float duration)
        {
            HealOverTimeEffect hot = new HealOverTimeEffect(this, healPerTick, tickInterval, duration);
            activeEffects.Add(hot);
        }

        protected virtual void ApplyStatModifier(StatType statType, float amount)
        {
            switch (statType)
            {
                case StatType.Attack:
                    currentAttack += amount;
                    break;
                case StatType.Defense:
                    currentDefense += amount;
                    break;
                case StatType.Speed:
                    currentSpeed += amount;
                    break;
            }
        }

        protected virtual void Die()
        {
            IsAlive = false;
            
            // Clear all active effects when agent dies
            activeEffects.Clear();
            
            // Notify UI of death
            OnDeath?.Invoke(this);
        }

        // Getters for UI display
        public float GetCurrentHealth() => currentHealth;
        public float GetMaxHealth() => maxHealth;
        public float GetAttack() => currentAttack;
        public float GetDefense() => currentDefense;
        public float GetSpeed() => currentSpeed;
        public List<Effect> GetActiveEffects() => activeEffects;
    }

    // Enum for stat types that can be buffed/debuffed
    public enum StatType
    {
        Attack,
        Defense,
        Speed
    }
}
