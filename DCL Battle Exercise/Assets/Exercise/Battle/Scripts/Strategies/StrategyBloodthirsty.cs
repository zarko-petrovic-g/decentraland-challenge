using UnityEngine;

public class StrategyBloodthirsty : UnitStrategy
{
    public StrategyBloodthirsty(UnitBase unit) : base(unit) { }

    public override void Update()
    {
        Vector3 position = unit.CachedTransform.position;

        bool enemyFound = unit.EnemyArmy.FindMinHealthUnit(position, unit.AttackRange, out UnitBase enemy);

        if(!enemyFound)
        {
            enemyFound = unit.EnemyArmy.GetClosestUnit(position, out float distance, out enemy);

            if(!enemyFound)
            {
                return;
            }
        }

        Vector3 toNearest = enemy.CachedTransform.position - position;
        toNearest.y = 0f;
        unit.Move(toNearest.normalized);
        unit.Attack(enemy);
    }
}