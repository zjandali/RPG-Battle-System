using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AstrobloxRPG
{
    /// <summary>
    /// Base class for all effects (buffs, debuffs, DoT, HoT)
    /// </summary>
    public abstract class Effect
    {
        protected Agent targetAgent;
        protected float duration;
        protected float remainingTime;
        
        public bool IsActive => remainingTime > 0;
        public float RemainingTime => remainingTime;
        public float TotalDuration => duration;
        
        public Effect(Agent target, float duration)
        {
            this.targetAgent = target;
            this.duration = duration;
            this.remainingTime = duration;
        }
        
        public virtual void UpdateEffect(float deltaTime)
        {
            remainingTime -= deltaTime;
            
            if (remainingTime <= 0)
            {
                OnEffectEnd();
            }
        }
        
        protected abstract void OnEffectEnd();
        
        public abstract string GetEffectDescription();
    }
    
    /// <summary>
    /// Buff effect that increases a stat for a duration
    /// </summary>
    public class BuffEffect : Effect
    {
        private StatType statType;
        private float amount;
        
        public BuffEffect(Agent target, StatType statType, float amount, float duration) : base(target, duration)
        {
            this.statType = statType;
            this.amount = amount;
        }
        
        protected override void OnEffectEnd()
        {
            // Reverse the buff by applying a negative amount
            switch (statType)
            {
                case StatType.Attack:
                    targetAgent.ApplyBuff(StatType.Attack, -amount, 0);
                    break;
                case StatType.Defense:
                    targetAgent.ApplyBuff(StatType.Defense, -amount, 0);
                    break;
                case StatType.Speed:
                    targetAgent.ApplyBuff(StatType.Speed, -amount, 0);
                    break;
            }
        }
        
        public override string GetEffectDescription()
        {
            return $"+{amount} {statType} ({remainingTime:F1}s)";
        }
    }
    
    /// <summary>
    /// Debuff effect that decreases a stat for a duration
    /// </summary>
    public class DebuffEffect : Effect
    {
        private StatType statType;
        private float amount;
        
        public DebuffEffect(Agent target, StatType statType, float amount, float duration) : base(target, duration)
        {
            this.statType = statType;
            this.amount = amount;
        }
        
        protected override void OnEffectEnd()
        {
            // Reverse the debuff by applying a positive amount
            switch (statType)
            {
                case StatType.Attack:
                    targetAgent.ApplyBuff(StatType.Attack, amount, 0);
                    break;
                case StatType.Defense:
                    targetAgent.ApplyBuff(StatType.Defense, amount, 0);
                    break;
                case StatType.Speed:
                    targetAgent.ApplyBuff(StatType.Speed, amount, 0);
                    break;
            }
        }
        
        public override string GetEffectDescription()
        {
            return $"-{amount} {statType} ({remainingTime:F1}s)";
        }
    }
    
    /// <summary>
    /// Damage over time effect
    /// </summary>
    public class DamageOverTimeEffect : Effect
    {
        private float damagePerTick;
        private float tickInterval;
        private float timeSinceLastTick;
        
        public DamageOverTimeEffect(Agent target, float damagePerTick, float tickInterval, float duration) : base(target, duration)
        {
            this.damagePerTick = damagePerTick;
            this.tickInterval = tickInterval;
            this.timeSinceLastTick = 0;
        }
        
        public override void UpdateEffect(float deltaTime)
        {
            base.UpdateEffect(deltaTime);
            
            if (!IsActive) return;
            
            timeSinceLastTick += deltaTime;
            
            if (timeSinceLastTick >= tickInterval)
            {
                targetAgent.TakeDamage(damagePerTick, null);
                timeSinceLastTick = 0;
            }
        }
        
        protected override void OnEffectEnd()
        {
            // Nothing special to do when DoT ends
        }
        
        public override string GetEffectDescription()
        {
            return $"DoT: {damagePerTick}/tick ({remainingTime:F1}s)";
        }
    }
    
    /// <summary>
    /// Heal over time effect
    /// </summary>
    public class HealOverTimeEffect : Effect
    {
        private float healPerTick;
        private float tickInterval;
        private float timeSinceLastTick;
        
        public HealOverTimeEffect(Agent target, float healPerTick, float tickInterval, float duration) : base(target, duration)
        {
            this.healPerTick = healPerTick;
            this.tickInterval = tickInterval;
            this.timeSinceLastTick = 0;
        }
        
        public override void UpdateEffect(float deltaTime)
        {
            base.UpdateEffect(deltaTime);
            
            if (!IsActive) return;
            
            timeSinceLastTick += deltaTime;
            
            if (timeSinceLastTick >= tickInterval)
            {
                targetAgent.Heal(healPerTick);
                timeSinceLastTick = 0;
            }
        }
        
        protected override void OnEffectEnd()
        {
            // Nothing special to do when HoT ends
        }
        
        public override string GetEffectDescription()
        {
            return $"HoT: {healPerTick}/tick ({remainingTime:F1}s)";
        }
    }
    
    // Add a utility class to contain the static methods
    public static class EffectFactory
    {
        // Move the static methods inside this class
        public static Effect CreateBuff(StatType statType, float amount, float duration, Agent target)
        {
            // Same duration and calculation for both players and enemies
            return new BuffEffect(target, statType, amount, duration);
        }

        public static Effect CreateDebuff(StatType statType, float amount, float duration, Agent target)
        {
            // Same duration and calculation for both players and enemies
            return new DebuffEffect(target, statType, amount, duration);
        }
    }
}
