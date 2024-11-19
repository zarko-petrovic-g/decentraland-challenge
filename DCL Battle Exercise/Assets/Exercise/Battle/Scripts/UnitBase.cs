using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class UnitBase : MonoBehaviour
{
    public Army army;

    [NonSerialized]
    public IArmyModel armyModel;

    protected float attackCooldown;
    private Vector3 lastPosition;
    public float health { get; protected set; }
    public float defense { get; protected set; }
    public float attack { get; protected set; }
    public float maxAttackCooldown { get; protected set; }
    public float postAttackDelay { get; protected set; }
    public float speed { get; protected set; } = 0.1f;

    // TODO get; private set; ? performance implications?
    [NonSerialized]
    public Transform CachedTransform;
    
    protected virtual void Awake()
    {
        CachedTransform = transform;
    }

    private void Update()
    {
        if(health < 0)
            return;

        List<UnitBase> allies = army.Units;
        List<UnitBase> enemies = army.EnemyArmy.Units;

        attackCooldown -= Time.deltaTime;
        EvadeOtherUnits();

        // TODO use strategy pattern
        switch(armyModel.strategy)
        {
            case ArmyStrategy.Defensive:
                UpdateDefensive(allies, enemies);
                break;
            case ArmyStrategy.Basic:
                UpdateBasic(allies, enemies);
                break;
        }

        var animator = GetComponentInChildren<Animator>();
        animator.SetFloat("MovementSpeed", (transform.position - lastPosition).magnitude / speed);
        lastPosition = transform.position;
    }

    public abstract void Attack(UnitBase enemy);


    protected abstract void UpdateDefensive(List<UnitBase> allies, List<UnitBase> enemies);
    protected abstract void UpdateBasic(List<UnitBase> allies, List<UnitBase> enemies);

    public virtual void Move(Vector3 delta)
    {
        if(attackCooldown > maxAttackCooldown - postAttackDelay)
            return;

        transform.position += delta * speed;
    }

    public virtual void Hit(GameObject sourceGo)
    {
        // TODO avoid all these getcomponents and cache the animator hash
        var source = sourceGo.GetComponent<UnitBase>();
        float sourceAttack = 0;

        if(source != null)
        {
            sourceAttack = source.attack;
        }
        else
        {
            var arrow = sourceGo.GetComponent<ArcherArrow>();
            sourceAttack = arrow.attack;
        }

        health -= Mathf.Max(sourceAttack - defense, 0);

        if(health < 0)
        {
            transform.forward = sourceGo.transform.position - transform.position;

            army.Remove(this);
            BattleInstantiator.instance.AllUnits.Remove(this);

            var animator = GetComponentInChildren<Animator>();
            animator?.SetTrigger("Death");
        }
        else
        {
            var animator = GetComponentInChildren<Animator>();
            animator?.SetTrigger("Hit");
        }
    }

    private void EvadeOtherUnits()
    {
        // TODO move clamp position out of this method, this should just evade units
        var battleInstantiator = BattleInstantiator.instance;
        Vector3 center = battleInstantiator.Center;

        Vector3 position = CachedTransform.position;
        float centerDist = Vector3.Distance(position, center);

        if(centerDist > 80.0f)
        {
            Vector3 toNearest = (center - position).normalized;
            CachedTransform.position -= toNearest * (80.0f - centerDist);
            return;
        }

        List<UnitBase> allUnits = battleInstantiator.AllUnits;
        int allUnitsCount = allUnits.Count;

        for(int index = 0; index < allUnitsCount; index++)
        {
            Vector3 otherUnitPosition = allUnits[index].CachedTransform.position;
            float dist = Vector3.Distance(position, otherUnitPosition);

            if(dist < 2f)
            {
                Vector3 toNearest = (otherUnitPosition - position).normalized;
                position -= toNearest * (2.0f - dist);
                CachedTransform.position = position;
            }
        }
    }

    public void OnDeathAnimFinished()
    {
        Destroy(gameObject);
    }
}