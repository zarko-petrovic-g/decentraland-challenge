using System.Collections.Generic;
using UnityEngine;

public class Archer : UnitBase
{
    public float attackRange = 20f;

    public ArcherArrow arrowPrefab;

    private void Awake()
    {
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

    public void OnDeathAnimFinished()
    {
        Destroy(gameObject);
    }

    protected override void UpdateDefensive(List<UnitBase> allies, List<UnitBase> enemies)
    {
        Vector3 enemyCenter = Utils.GetCenter(enemies);
        float distToEnemyX = Mathf.Abs(enemyCenter.x - transform.position.x);

        if(distToEnemyX > attackRange)
        {
            if(enemyCenter.x < transform.position.x)
                Move(Vector3.left);

            if(enemyCenter.x > transform.position.x)
                Move(Vector3.right);
        }

        float distToNearest = Utils.GetNearestEnemy(gameObject, enemies, out UnitBase nearestEnemy);

        if(nearestEnemy == null)
            return;

        if(distToNearest < attackRange)
        {
            Vector3 toNearest = (nearestEnemy.transform.position - transform.position).normalized;
            toNearest.Scale(new Vector3(1, 0, 1));

            Vector3 flank = Quaternion.Euler(0, 90, 0) * toNearest;
            Move(-(toNearest + flank).normalized);
        }
        else
        {
            Vector3 toNearest = (nearestEnemy.transform.position - transform.position).normalized;
            toNearest.Scale(new Vector3(1, 0, 1));
            Move(toNearest.normalized);
        }

        Attack(nearestEnemy);
    }

    protected override void UpdateBasic(List<UnitBase> allies, List<UnitBase> enemies)
    {
        Utils.GetNearestEnemy(gameObject, enemies, out UnitBase nearestEnemy);

        if(nearestEnemy == null)
            return;

        Vector3 toNearest = (nearestEnemy.transform.position - transform.position).normalized;
        toNearest.Scale(new Vector3(1, 0, 1));
        Move(toNearest.normalized);

        Attack(nearestEnemy);
    }
}