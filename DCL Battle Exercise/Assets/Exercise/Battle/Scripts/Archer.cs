using UnityEngine;

public class Archer : UnitBase
{
    // TODO move to a SO
    public float attackRange = 20f;

    public ArcherArrow arrowPrefab;

    public override ArmyStrategy ArmyStrategy
    {
        set
        {
            UnitStrategy = value switch
            {
                ArmyStrategy.Basic => new ArcherStrategyBasic(this),
                ArmyStrategy.Defensive => new ArcherStrategyDefensive(this),
                _ => throw new System.ArgumentOutOfRangeException()
            };
        }
    }

    protected override void Awake()
    {
        base.Awake();

        health = 5;
        defense = 0;
        attack = 10;
        maxAttackCooldown = 5f;
        postAttackDelay = 1f;
    }

    public override void Attack(UnitBase enemy)
    {
        if(attackCooldown > 0)
            return;

        if(Vector3.Distance(CachedTransform.position, enemy.CachedTransform.position) > attackRange)
            return;

        attackCooldown = maxAttackCooldown;
        ArcherArrow arrow = Instantiate(arrowPrefab, CachedTransform.position, Quaternion.identity);
        arrow.Target = enemy.CachedTransform.position;
        arrow.Attack = attack;
        arrow.EnemyArmy = EnemyArmy;

        if(hasAnimator)
        {
            animator.SetTrigger(AttackTriggerId);
        }

        arrow.Color = Color;
    }
}