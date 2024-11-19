using System.Collections.Generic;
using UnityEngine;

public class Archer : UnitBase
{
    public float attackRange = 20f;

    public ArcherArrow arrowPrefab;

    protected override void Awake()
    {
        base.Awake();
        
        health = 5;
        defense = 0;
        attack = 10;
        maxAttackCooldown = 5f;
        postAttackDelay = 1f;
    }

    public override void Attack(UnitBase enemy)
    {
        if(attackCooldown > 0)
            return;

        if(Vector3.Distance(transform.position, enemy.transform.position) > attackRange)
            return;

        attackCooldown = maxAttackCooldown;
        GameObject arrow = Instantiate(arrowPrefab.gameObject);
        arrow.GetComponent<ArcherArrow>().target = enemy.transform.position;
        arrow.GetComponent<ArcherArrow>().attack = attack;
        arrow.GetComponent<ArcherArrow>().army = army;
        arrow.transform.position = transform.position;

        var animator = GetComponentInChildren<Animator>();
        animator?.SetTrigger("Attack");

        if(army == BattleInstantiator.instance.Army1)
            arrow.GetComponent<Renderer>().material.color = BattleInstantiator.instance.Army1Color;
        else
            arrow.GetComponent<Renderer>().material.color = BattleInstantiator.instance.Army2Color;
    }

    protected override void UpdateDefensive(List<UnitBase> allies, List<UnitBase> enemies)
    {
        // TODO get rid of this GetCenter call, we can use army.Center and improve performance
        Vector3 enemyCenter = Utils.GetCenter(enemies);
        Vector3 position = CachedTransform.position;
        float distToEnemyX = Mathf.Abs(enemyCenter.x - position.x);

        if(distToEnemyX > attackRange)
        {
            if(enemyCenter.x < position.x)
                Move(Vector3.left);

            if(enemyCenter.x > position.x)
                Move(Vector3.right);
        }

        bool enemyFound = Utils.GetNearestEnemy(position, enemies, out float distToNearest, out UnitBase nearestEnemy);

        if(!enemyFound) return;

        Vector3 enemyPosition = nearestEnemy.CachedTransform.position;

        if(distToNearest < attackRange)
        {
            Vector3 toNearest = (enemyPosition - position).normalized;
            toNearest.y = 0;

            Vector3 flank = Quaternion.Euler(0, 90, 0) * toNearest;
            Move(-(toNearest + flank).normalized);
        }
        else
        {
            Vector3 toNearest = (enemyPosition - position).normalized;
            toNearest.Scale(new Vector3(1, 0, 1));
            Move(toNearest.normalized);
        }

        Attack(nearestEnemy);
    }

    protected override void UpdateBasic(List<UnitBase> allies, List<UnitBase> enemies)
    {
        Vector3 position = CachedTransform.position;
        
        bool enemyFound = Utils.GetNearestEnemy(position, enemies, out _, out UnitBase nearestEnemy);

        if(!enemyFound) return;

        // TODO probably two normalization are not needed
        Vector3 toNearest = (nearestEnemy.CachedTransform.position - position).normalized;
        toNearest.y = 0;
        Move(toNearest.normalized);

        Attack(nearestEnemy);
    }
}