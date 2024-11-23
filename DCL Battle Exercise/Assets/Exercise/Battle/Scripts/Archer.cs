using System;
using UnityEngine;

public class Archer : UnitBase
{
    [SerializeField]
    private ArcherArrow arrowPrefab;

    public ArcherArrow ArrowPrefab => arrowPrefab;

    protected ArcherStats ArcherStats => (ArcherStats)stats;

    public override ArmyStrategy ArmyStrategy
    {
        set
        {
            UnitStrategy = value switch
            {
                ArmyStrategy.Basic => new ArcherStrategyBasic(this),
                ArmyStrategy.Defensive => new ArcherStrategyDefensive(this),
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
        var arrow = Battle.Pool.Get(PoolableCategory.ArcherArrow) as ArcherArrow;
        arrow.Init(position, enemy.CachedTransform.position, AttackDamage, EnemyArmy, ArcherStats.arrowSpeed, Color,
            Battle.Pool);

        if(hasAnimator)
        {
            animator.SetTrigger(AttackTriggerId);
        }
    }
}