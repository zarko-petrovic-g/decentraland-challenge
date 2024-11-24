using UnityEngine;

public class WarriorStrategyDefensive : UnitStrategy
{
    private const float MaxEnemyCenterDistance = 20f;

    public WarriorStrategyDefensive(UnitBase unit) : base(unit) { }

    public override void Update()
    {
        // TODO since this moves towards the enemy army, and then it moves towards the nearest enemy
        // it can happen that the unit moves more than it's speed in one frame?
        unit.MoveTowardsEnemyArmy(MaxEnemyCenterDistance);

        Vector3 position = unit.CachedTransform.position;

        bool enemyFound = unit.EnemyArmy.GetClosestUnit(position, out _, out UnitBase enemy);

        if(!enemyFound)
        {
            return;
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