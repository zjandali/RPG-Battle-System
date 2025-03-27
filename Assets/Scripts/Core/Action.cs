using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AstrobloxRPG
{
    /// <summary>
    /// Base class for all battle actions
    /// </summary>
    public abstract class Action
    {
        protected Agent source;
        protected Agent target;
        
        public Action(Agent source, Agent target)
        {
            this.source = source;
            this.target = target;
        }
        
        public abstract void Execute();
        public abstract string GetActionDescription();
    }
    
    /// <summary>
    /// Direct damage action
    /// </summary>
    public class DamageAction : Action
    {
        private float damageAmount;
        
        public DamageAction(Agent source, Agent target, float damageAmount) : base(source, target)
        {
            this.damageAmount = damageAmount;
        }
        
        public override void Execute()
        {
            if (target != null && target.IsAlive)
            {
                target.TakeDamage(damageAmount);
            }
        }
        
        public override string GetActionDescription()
        {
            return $"{source.gameObject.name} attacks {target.gameObject.name} for {damageAmount} damage";
        }
    }
    
    /// <summary>
    /// Direct heal action
    /// </summary>
    public class HealAction : Action
    {
        private float healAmount;
        
        public HealAction(Agent source, Agent target, float healAmount) : base(source, target)
        {
            this.healAmount = healAmount;
        }
        
        public override void Execute()
        {
            if (target != null && target.IsAlive)
            {
                target.Heal(healAmount);
            }
        }
        
        public override string GetActionDescription()
        {
            return $"{source.gameObject.name} heals {target.gameObject.name} for {healAmount} health";
        }
    }
    
    /// <summary>
    /// Apply damage over time effect
    /// </summary>
    public class ApplyDamageOverTimeAction : Action
    {
        private float damagePerTick;
        private float tickInterval;
        private float duration;
        
        public ApplyDamageOverTimeAction(Agent source, Agent target, float damagePerTick, float tickInterval, float duration) : base(source, target)
        {
            this.damagePerTick = damagePerTick;
            this.tickInterval = tickInterval;
            this.duration = duration;
        }
        
        public override void Execute()
        {
            if (target != null && target.IsAlive)
            {
                target.ApplyDamageOverTime(damagePerTick, tickInterval, duration);
            }
        }
        
        public override string GetActionDescription()
        {
            return $"{source.gameObject.name} applies DoT to {target.gameObject.name} for {damagePerTick} damage every {tickInterval}s for {duration}s";
        }
    }
    
    /// <summary>
    /// Apply heal over time effect
    /// </summary>
    public class ApplyHealOverTimeAction : Action
    {
        private float healPerTick;
        private float tickInterval;
        private float duration;
        
        public ApplyHealOverTimeAction(Agent source, Agent target, float healPerTick, float tickInterval, float duration) : base(source, target)
        {
            this.healPerTick = healPerTick;
            this.tickInterval = tickInterval;
            this.duration = duration;
        }
        
        public override void Execute()
        {
            if (target != null && target.IsAlive)
            {
                target.ApplyHealOverTime(healPerTick, tickInterval, duration);
            }
        }
        
        public override string GetActionDescription()
        {
            return $"{source.gameObject.name} applies HoT to {target.gameObject.name} for {healPerTick} healing every {tickInterval}s for {duration}s";
        }
    }
    
    /// <summary>
    /// Apply buff effect
    /// </summary>
    public class ApplyBuffAction : Action
    {
        private StatType statType;
        private float amount;
        private float duration;
        
        public ApplyBuffAction(Agent source, Agent target, StatType statType, float amount, float duration) : base(source, target)
        {
            this.statType = statType;
            this.amount = amount;
            this.duration = duration;
        }
        
        public override void Execute()
        {
            if (target != null && target.IsAlive)
            {
                target.ApplyBuff(statType, amount, duration);
            }
        }
        
        public override string GetActionDescription()
        {
            return $"{source.gameObject.name} buffs {target.gameObject.name}'s {statType} by {amount} for {duration}s";
        }
    }
    
    /// <summary>
    /// Apply debuff effect
    /// </summary>
    public class ApplyDebuffAction : Action
    {
        private StatType statType;
        private float amount;
        private float duration;
        
        public ApplyDebuffAction(Agent source, Agent target, StatType statType, float amount, float duration) : base(source, target)
        {
            this.statType = statType;
            this.amount = amount;
            this.duration = duration;
        }
        
        public override void Execute()
        {
            if (target != null && target.IsAlive)
            {
                target.ApplyDebuff(statType, amount, duration);
            }
        }
        
        public override string GetActionDescription()
        {
            return $"{source.gameObject.name} debuffs {target.gameObject.name}'s {statType} by {amount} for {duration}s";
        }
    }
}
