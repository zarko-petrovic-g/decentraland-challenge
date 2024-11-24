using UnityEngine;

public class StrategyBasic : UnitStrategy
{
    public StrategyBasic(UnitBase unit) : base(unit) { }

    public override void Update()
    {
        Vector3 position = unit.CachedTransform.position;

        bool enemyFound = unit.EnemyArmy.GetClosestUnit(position, out _, out UnitBase enemy);

        if(!enemyFound)
        {
            return;
        }

        Vector3 toNearest = enemy.CachedTransform.position - position;
        toNearest.y = 0f;
        unit.Move(toNearest.normalized);
        unit.Attack(enemy);
    }
}