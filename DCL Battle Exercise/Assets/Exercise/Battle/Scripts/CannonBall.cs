using System;
using UnityEngine;

public class CannonBall : Projectile
{
    private const float SpeedMultiplier = 0.01f;
    private float damageRadius;

    [NonSerialized]
    public float Gravity;

    private UnitBase[] hits;
    private float speedVertical;

    public int MaxHits
    {
        set => hits = new UnitBase[value];
    }

    public override PoolableCategory Category => PoolableCategory.CannonBall;

    private void Update()
    {
        Vector3 position = CachedTransform.position;
        position += movement;
        speedVertical -= Gravity * SpeedMultiplier;
        position.y += speedVertical;
        CachedTransform.position = position;

        if(position.y < -speedVertical)
        {
            int hitCount = enemyArmy.GetUnits(position, damageRadius, hits);

            for(int i = 0; i < hitCount; i++)
            {
                hits[i].Hit(attack, position);
            }

            pool.Return(this);
        }
    }

    public void Init(Vector3 position, Vector3 target, float attackDamage, float damageRadius, Army enemyArmy,
        Color color, Pool pool)
    {
        float distance = Vector3.Distance(position, target);
        speedVertical = Mathf.Sqrt(distance * Gravity * SpeedMultiplier / 2f);
        // we are firing at 45 degrees so the horizontal speed is the same as the vertical speed
        float speedHorizontal = speedVertical;
        this.damageRadius = damageRadius;
        base.Init(position, target, attackDamage, enemyArmy, speedHorizontal, color, pool);
    }
}