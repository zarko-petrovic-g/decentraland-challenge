using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcherStrategyBasic : UnitStrategy
{
    public ArcherStrategyBasic(UnitBase unit) : base(unit) { }

    public override void Update()
    {
        Vector3 position = unit.CachedTransform.position;

        bool enemyFound = Utils.GetNearestEnemy(position, unit.EnemyArmy.Units, out _, out UnitBase nearestEnemy);

        if(!enemyFound) return;

        // TODO probably two normalization are not needed
        Vector3 toNearest = (nearestEnemy.CachedTransform.position - position).normalized;
        toNearest.y = 0;
        unit.Move(toNearest.normalized);

        unit.Attack(nearestEnemy);
    }
}
