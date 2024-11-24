using UnityEngine;

public class Cannon : RangedUnit
{
    [SerializeField]
    private CannonBall cannonBallPrefab;

    public CannonBall CannonBallPrefab => cannonBallPrefab;

    public CannonStats CannonStats => (CannonStats)stats;

    protected override void LaunchProjectile(Vector3 position, Vector3 targetPosition)
    {
        var cannonBall = Battle.Pool.Get(PoolableCategory.CannonBall) as CannonBall;
        cannonBall.Init(position, targetPosition, AttackDamage, CannonStats.damageRadius, EnemyArmy,
            Color, Battle.Pool);
    }
}