using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcherStrategyDefensive : UnitStrategy
{
    // TODO move to a SO
    public float attackRange = 20f;
    
    public ArcherStrategyDefensive(UnitBase unit) : base(unit) { }
    
    public override void Update()
    {
        Vector3 enemyCenter = unit.EnemyArmy.Center;
        Vector3 position = unit.CachedTransform.position;
        float distToEnemyX = Mathf.Abs(enemyCenter.x - position.x);

        if(distToEnemyX > attackRange)
        {
            if(enemyCenter.x < position.x)
                unit.Move(Vector3.left);

            if(enemyCenter.x > position.x)
                unit.Move(Vector3.right);
        }

        bool enemyFound =
            Utils.GetNearestEnemy(position, unit.EnemyArmy.Units, out float distToNearest, out UnitBase nearestEnemy);

        if(!enemyFound) return;

        Vector3 enemyPosition = nearestEnemy.CachedTransform.position;

        if(distToNearest < attackRange)
        {
            Vector3 toNearest = (enemyPosition - position).normalized;
            toNearest.y = 0;

            Vector3 flank = Quaternion.Euler(0, 90, 0) * toNearest;
            unit.Move(-(toNearest + flank).normalized);
        }
        else
        {
            Vector3 toNearest = (enemyPosition - position).normalized;
            toNearest.y = 0f;
            unit.Move(toNearest.normalized);
        }

        unit.Attack(nearestEnemy);
    }
}
