using UnityEngine;

public class ArcherArrow : Projectile
{
    public override PoolableCategory Category => PoolableCategory.ArcherArrow;

    public void Update()
    {
        Vector3 position = CachedTransform.position;
        position += movement;
        CachedTransform.position = position;

        if(enemyArmy.FindUnit(position, speed, out UnitBase unit))
        {
            unit.Hit(attack, position);
            pool.Return(this);
            return;
        }

        if(Vector3.Distance(position, target) < speed)
        {
            pool.Return(this);
        }
    }

    public new void Init(Vector3 position, Vector3 target, float attack, Army enemyArmy, float speed, Color color,
        Pool pool)
    {
        base.Init(position, target, attack, enemyArmy, speed, color, pool);
    }
}