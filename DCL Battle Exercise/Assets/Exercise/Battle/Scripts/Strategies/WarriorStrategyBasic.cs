using UnityEngine;

public class WarriorStrategyBasic : UnitStrategy
{
    public WarriorStrategyBasic(UnitBase unit) : base(unit) { }

    public override void Update()
    {
        Vector3 position = unit.CachedTransform.position;
        bool enemyFound = unit.EnemyArmy.GetNearestUnit(position, out _, out UnitBase enemy);
        if(!enemyFound)
        {
            return;
        }

        // TODO probably two normalizations are not needed
        Vector3 toNearest = (enemy.CachedTransform.position - position).normalized;
        toNearest.y = 0f;
        unit.Move(toNearest.normalized);

        unit.Attack(enemy);
    }
}