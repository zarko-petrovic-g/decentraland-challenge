using UnityEngine;

public class Archer : RangedUnit
{
    [SerializeField]
    private ArcherArrow arrowPrefab;

    public ArcherArrow ArrowPrefab => arrowPrefab;

    public ArcherStats ArcherStats => (ArcherStats)stats;

    protected override void LaunchProjectile(Vector3 position, Vector3 targetPosition)
    {
        var arrow = Battle.Pool.Get(PoolableCategory.ArcherArrow) as ArcherArrow;
        arrow.Init(position, targetPosition, AttackDamage, EnemyArmy, ArcherStats.arrowSpeed, Color, Battle.Pool);
    }
}