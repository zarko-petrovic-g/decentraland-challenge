using System;
using UnityEngine;

public abstract class UnitBase : MonoBehaviour
{
    // TODO move to a SO
    private const float BattleRadius = 80f;
    protected readonly static int AttackTriggerId = Animator.StringToHash("Attack");
    private readonly static int MovementSpeedParamId = Animator.StringToHash("MovementSpeed");
    private readonly static int DeathTriggerId = Animator.StringToHash("Death");
    private readonly static int HitTriggerId = Animator.StringToHash("Hit");

    protected Animator animator;

    public Army Army;

    protected float attackCooldown;

    public float AttackCooldown => attackCooldown;
    
    [NonSerialized]
    public Battle Battle;

    public Army EnemyArmy;
    protected bool hasAnimator;
    private Vector3 lastPosition;

    protected new Renderer renderer;
    public float health { get; protected set; }
    public float defense { get; protected set; }
    public float attack { get; protected set; }

    public float maxAttackCooldown { get; protected set; }

    // TODO move postAttackDelay to Archer?
    public float postAttackDelay { get; protected set; }
    public float speed { get; protected set; } = 0.1f;

    // TODO check performance implications compared to a field
    public Transform CachedTransform { get; private set; }
    
    protected IUnitStrategy UnitStrategy;

    public Color Color
    {
        set => renderer.material.color = value;
        get => renderer.material.color;
    }

    public abstract ArmyStrategy ArmyStrategy { set; }
    
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

        attackCooldown -= Time.deltaTime;
        bool positionChanged = ClampPosition();

        if(!positionChanged)
        {
            Battle.EvadeOtherUnits(this);
        }

        UnitStrategy?.Update();

        animator.SetFloat(MovementSpeedParamId, (CachedTransform.position - lastPosition).magnitude / speed);
        lastPosition = CachedTransform.position;
    }

    public event Action<UnitBase> OnDeath;

    public abstract void Attack(UnitBase enemy);
    
    public virtual void Move(Vector3 delta)
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
            Army.Remove(this);
            OnDeath?.Invoke(this);
            animator.SetTrigger(DeathTriggerId);
        }
        else
        {
            animator.SetTrigger(HitTriggerId);
        }
    }

    private bool ClampPosition()
    {
        Vector3 center = Battle.Center;

        Vector3 position = CachedTransform.position;
        float centerDist = Vector3.Distance(position, center);

        if(centerDist > BattleRadius)
        {
            Vector3 toNearest = (center - position).normalized;
            CachedTransform.position -= toNearest * (BattleRadius - centerDist);
            return true;
        }

        return false;
    }

    public void OnDeathAnimFinished()
    {
        Destroy(gameObject);
    }
}