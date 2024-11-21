using UnityEngine;

public class ArcherStrategyDefensive : UnitStrategy
{
    public ArcherStrategyDefensive(UnitBase unit) : base(unit) { }

    public override void Update()
    {
        // TODO since this moves towards the enemy army, and then it moves towards the nearest enemy
        // it can happen that the unit moves more than it's speed in one frame?
        unit.MoveTowardsEnemyArmy(unit.AttackRange);

        Vector3 position = unit.CachedTransform.position;

        bool enemyFound = unit.EnemyArmy.GetNearestUnit(position, out float distToNearest, out UnitBase nearestEnemy);
        
        if(!enemyFound)
        {
            return;
        }

        Vector3 enemyPosition = nearestEnemy.CachedTransform.position;

        Vector3 toNearest = (enemyPosition - position).normalized;
        toNearest.y = 0;
        Vector3 movement;

        if(distToNearest < unit.AttackRange)
        {
            Vector3 flank = Quaternion.Euler(0, 90, 0) * toNearest;
            movement = -(toNearest + flank);
        }
        else
        {
            movement = toNearest;
        }

        unit.Move(movement.normalized);
        unit.Attack(nearestEnemy);
    }
}