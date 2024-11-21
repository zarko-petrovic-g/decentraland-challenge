using System;
using UnityEngine;

public class Warrior : UnitBase
{
    public override ArmyStrategy ArmyStrategy
    {
        set
        {
            UnitStrategy = value switch
            {
                ArmyStrategy.Basic => new WarriorStrategyBasic(this),
                ArmyStrategy.Defensive => new WarriorStrategyDefensive(this),
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

        if(Vector3.Distance(position, enemy.CachedTransform.position) > AttackRange)
        {
            return;
        }

        attackCooldown = MaxAttackCooldown;

        if(hasAnimator)
        {
            animator.SetTrigger(AttackTriggerId);
        }

        enemy.Hit(AttackDamage, position);
    }
}