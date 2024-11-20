using UnityEngine;

public class WarriorStrategyDefensive : UnitStrategy
{
    private const float MaxEnemyCenterDistance = 20f;

    public WarriorStrategyDefensive(UnitBase unit) : base(unit) { }

    public override void Update()
    {
        Vector3 enemyCenter = unit.EnemyArmy.Center;

        Vector3 position = unit.CachedTransform.position;

        if(Mathf.Abs(enemyCenter.x - position.x) > MaxEnemyCenterDistance)
        {
            if(enemyCenter.x < position.x)
            {
                unit.Move(Vector3.left);
            }

            if(enemyCenter.x > position.x)
            {
                unit.Move(Vector3.right);
            }
        }

        bool enemyFound = Utils.GetNearestEnemy(position, unit.EnemyArmy.Units, out _, out UnitBase enemy);

        if(!enemyFound)
        {
            return;
        }

        Vector3 enemyPosition = enemy.CachedTransform.position;

        if(unit.AttackCooldown <= 0)
        {
            unit.Move((enemyPosition - position).normalized);
        }
        else
        {
            unit.Move((position - enemyPosition).normalized);
        }

        unit.Attack(enemy);
    }
}