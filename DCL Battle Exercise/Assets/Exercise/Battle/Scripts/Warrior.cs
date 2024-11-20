using System;
using UnityEngine;

public class Warrior : UnitBase
{
    [NonSerialized]
    public float attackRange = 2.5f;

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

    protected override void Awake()
    {
        base.Awake();

        health = 50;
        defense = 5;
        attack = 20;
        maxAttackCooldown = 1f;
        postAttackDelay = 0;
    }

    public override void Attack(UnitBase enemy)
    {
        if(attackCooldown > 0)
            return;

        Vector3 position = CachedTransform.position;

        if(Vector3.Distance(position, enemy.CachedTransform.position) > attackRange)
            return;

        attackCooldown = maxAttackCooldown;

        if(hasAnimator)
        {
            animator.SetTrigger(AttackTriggerId);
        }

        enemy.Hit(attack, position);
    }
}