using System;
using UnityEngine;

public abstract class RangedUnit : UnitBase
{
    public override ArmyStrategy ArmyStrategy
    {
        set
        {
            UnitStrategy = value switch
            {
                ArmyStrategy.Basic => new StrategyBasic(this),
                ArmyStrategy.Defensive => new RangedStrategyDefensive(this),
                ArmyStrategy.Bloodthirsty => new StrategyBloodthirsty(this),
                ArmyStrategy.Cowardly => new RangedStrategyCowardly(this),
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }

    public override void Attack(UnitBase enemy)
    {
        if(attackCooldown > 0)
        {
            return;
        }

        Vector3 position = CachedTransform.position;
        Vector3 targetPosition = enemy.CachedTransform.position;

        if(Vector3.Distance(position, targetPosition) > AttackRange)
        {
            return;
        }

        attackCooldown = MaxAttackCooldown;

        LaunchProjectile(position, targetPosition);

        if(hasAnimator)
        {
            animator.SetTrigger(AttackTriggerId);
        }
    }

    protected abstract void LaunchProjectile(Vector3 position, Vector3 targetPosition);
}