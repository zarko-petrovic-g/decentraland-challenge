using System;
using System.Collections.Generic;
using UnityEngine;

public class Warrior : UnitBase
{
    [NonSerialized]
    public float attackRange = 2.5f;

    protected override void Awake()
    {
        base.Awake();
        
        health = 50;
        defense = 5;
        attack = 20;
        maxAttackCooldown = 1f;
        postAttackDelay = 0;
    }

    public override void Attack(UnitBase target)
    {
        if(attackCooldown > 0)
            return;

        Vector3 position = CachedTransform.position;
       
        if(Vector3.Distance(position, target.CachedTransform.position) > attackRange)
            return;

        attackCooldown = maxAttackCooldown;

        if(hasAnimator)
        {
            animator.SetTrigger(AttackTriggerId);
        }

        target.Hit(attack, position);
    }

    protected override void UpdateDefensive(List<UnitBase> allies, List<UnitBase> enemies)
    {
        Vector3 enemyCenter = Utils.GetCenter(enemies);

        Vector3 position = transform.position;

        if(Mathf.Abs(enemyCenter.x - position.x) > 20)
        {
            if(enemyCenter.x < position.x)
                Move(Vector3.left);

            if(enemyCenter.x > position.x)
                Move(Vector3.right);
        }

        bool enemyFound = Utils.GetNearestEnemy(position, enemies, out _, out UnitBase enemy);

        if(!enemyFound) return;

        Vector3 enemyPosition = enemy.transform.position;

        if(attackCooldown <= 0)
        {
            Move((enemyPosition - position).normalized);
        }
        else
        {
            Move((position - enemyPosition).normalized);
        }

        Attack(enemy);
    }

    protected override void UpdateBasic(List<UnitBase> allies, List<UnitBase> enemies)
    {
        Vector3 position = transform.position;
        bool enemyFound = Utils.GetNearestEnemy(position, enemies, out _, out UnitBase nearestEnemy);

        if(!enemyFound) return;

        // TODO probably two normalizations are not needed
        Vector3 toNearest = (nearestEnemy.CachedTransform.position - position).normalized;
        toNearest.y = 0f;
        Move(toNearest.normalized);

        Attack(nearestEnemy);
    }
}