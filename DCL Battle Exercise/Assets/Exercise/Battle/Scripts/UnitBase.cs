using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class UnitBase : MonoBehaviour
{
    protected readonly static int AttackTriggerId = Animator.StringToHash("Attack");
    private readonly static int MovementSpeedParamId = Animator.StringToHash("MovementSpeed");
    private readonly static int DeathTriggerId = Animator.StringToHash("Death");
    private readonly static int HitTriggerId = Animator.StringToHash("Hit");

    public Army army;

    [NonSerialized]
    public IArmyModel armyModel;

    protected float attackCooldown;
    private Vector3 lastPosition;
    public float health { get; protected set; }
    public float defense { get; protected set; }
    public float attack { get; protected set; }
    public float maxAttackCooldown { get; protected set; }
    // TODO move postAttackDelay to Archer?
    public float postAttackDelay { get; protected set; }
    public float speed { get; protected set; } = 0.1f;

    // TODO check performance implications compared to a field
    public Transform CachedTransform { get; private set; }

    protected Animator animator;
    protected bool hasAnimator;

    protected new Renderer renderer;
    
    public Color Color
    {
        set => renderer.material.color = value;
        get => renderer.material.color;
    }
    
    protected virtual void Awake()
    {
        CachedTransform = transform;
        animator = GetComponentInChildren<Animator>();
        hasAnimator = animator != null;
        renderer = GetComponentInChildren<Renderer>();
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

        animator.SetFloat(MovementSpeedParamId, (CachedTransform.position - lastPosition).magnitude / speed);
        lastPosition = CachedTransform.position;
    }

    protected abstract void UpdateDefensive(List<UnitBase> allies, List<UnitBase> enemies);
    protected abstract void UpdateBasic(List<UnitBase> allies, List<UnitBase> enemies);

    protected virtual void Move(Vector3 delta)
    {
        if(attackCooldown > maxAttackCooldown - postAttackDelay)
            return;

        transform.position += delta * speed;
    }

    public virtual void Hit(float damage, Vector3 attackerPosition)
    {
        health -= Mathf.Max(damage - defense, 0);

        if(health < 0)
        {
            CachedTransform.forward = attackerPosition - CachedTransform.position;

            army.Remove(this);
            BattleInstantiator.instance.AllUnits.Remove(this);

            animator.SetTrigger(DeathTriggerId);
        }
        else
        {
            animator.SetTrigger(HitTriggerId);
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