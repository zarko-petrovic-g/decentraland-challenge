using UnityEngine;

public class WarriorStrategyCowardly : UnitStrategy
{
    public WarriorStrategyCowardly(UnitBase unit) : base(unit) { }

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

        Vector3 enemyPosition = enemy.CachedTransform.position;
        Vector3 movement = enemyPosition - position;

        if(unit.AttackCooldown > 0)
        {
            movement *= -1f;
        }

        unit.Move(movement.normalized);
        unit.Attack(enemy);
    }
}