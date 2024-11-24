using UnityEngine;

public class RangedStrategyCowardly : UnitStrategy
{
    public RangedStrategyCowardly(UnitBase unit) : base(unit) { }

    public override void Update()
    {
        Vector3 position = unit.CachedTransform.position;

        bool enemyFound = unit.EnemyArmy.FindMinHealthUnit(position, unit.AttackRange, out UnitBase enemy);
        Vector3 enemyPosition;
        float distance;

        if(enemyFound)
        {
            enemyPosition = enemy.CachedTransform.position;
            distance = Vector3.Distance(position, enemyPosition);
        }
        else
        {
            enemyFound = unit.EnemyArmy.GetClosestUnit(position, out distance, out enemy);

            if(enemyFound)
            {
                enemyPosition = enemy.CachedTransform.position;
            }
            else
            {
                return;
            }
        }

        Vector3 toNearest = (enemyPosition - position).normalized;
        toNearest.y = 0;
        Vector3 movement;

        if(distance < unit.AttackRange)
        {
            Vector3 flank = Quaternion.Euler(0, 90, 0) * toNearest;
            movement = -(toNearest + flank);
        }
        else
        {
            movement = toNearest;
        }

        unit.Move(movement.normalized);
        unit.Attack(enemy);
    }
}