using UnityEngine;

public class Archer : UnitBase
{
    [SerializeField]
    private ArcherArrow arrowPrefab;

    protected ArcherStats ArcherStats => (ArcherStats)stats;

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
    
    public override void Attack(UnitBase enemy)
    {
        if(attackCooldown > 0)
            return;

        if(Vector3.Distance(CachedTransform.position, enemy.CachedTransform.position) > AttackRange)
            return;

        attackCooldown = MaxAttackCooldown;
        ArcherArrow arrow = Instantiate(arrowPrefab, CachedTransform.position, Quaternion.identity);
        arrow.Target = enemy.CachedTransform.position;
        arrow.Attack = AttackDamage;
        arrow.EnemyArmy = EnemyArmy;
        arrow.Speed = ArcherStats.arrowSpeed;

        if(hasAnimator)
        {
            animator.SetTrigger(AttackTriggerId);
        }

        arrow.Color = Color;
    }
}