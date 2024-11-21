using System;
using UnityEngine;

public abstract class UnitBase : MonoBehaviour
{
    protected readonly static int AttackTriggerId = Animator.StringToHash("Attack");
    private readonly static int MovementSpeedParamId = Animator.StringToHash("MovementSpeed");
    private readonly static int DeathTriggerId = Animator.StringToHash("Death");
    private readonly static int HitTriggerId = Animator.StringToHash("Hit");

    [SerializeField]
    protected UnitStats stats;

    protected Animator animator;

    public Army Army;

    protected float attackCooldown;

    [NonSerialized]
    public Battle Battle;

    public Army EnemyArmy;
    protected bool hasAnimator;
    private Vector3 lastPosition;

    protected new Renderer renderer;

    protected IUnitStrategy UnitStrategy;

    public float AttackCooldown => attackCooldown;

    public Transform CachedTransform { get; private set; }

    public float CurrentHealth { get; private set; }

    public Color Color
    {
        set => renderer.material.color = value;
        get => renderer.material.color;
    }

    public abstract ArmyStrategy ArmyStrategy { set; }

    public float AttackRange => stats.attackRange;
    public float Speed => stats.speed;
    public float Defense => stats.defense;
    public float MaxAttackCooldown => stats.maxAttackCooldown;
    public float PostAttackDelay => stats.postAttackDelay;
    public float MaxHealth => stats.health;
    public float AttackDamage => stats.attack;


    protected virtual void Awake()
    {
        CachedTransform = transform;
        animator = GetComponentInChildren<Animator>();
        hasAnimator = animator != null;
        renderer = GetComponentInChildren<Renderer>();
        CurrentHealth = MaxHealth;
    }

    private void Update()
    {
        if(CurrentHealth < 0)
        {
            return;
        }

        attackCooldown -= Time.deltaTime;
        bool positionChanged = Battle.ClampPosition(this);

        if(!positionChanged)
        {
            Battle.EvadeOtherUnits(this);
        }

        UnitStrategy?.Update();

        animator.SetFloat(MovementSpeedParamId, (CachedTransform.position - lastPosition).magnitude / Speed);
        lastPosition = CachedTransform.position;
    }

    public event Action<UnitBase> OnDeath;

    public abstract void Attack(UnitBase enemy);

    public virtual void Move(Vector3 delta)
    {
        if(attackCooldown > MaxAttackCooldown - PostAttackDelay)
        {
            return;
        }

        transform.position += delta * Speed;
    }

    public virtual void Hit(float damage, Vector3 attackerPosition)
    {
        CurrentHealth -= Mathf.Max(damage - Defense, 0);

        if(CurrentHealth < 0)
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

    public void MoveTowardsEnemyArmy(float desiredDistance)
    {
        Vector3 enemyCenter = EnemyArmy.Center;

        Vector3 position = CachedTransform.position;

        if(Mathf.Abs(enemyCenter.x - position.x) > desiredDistance)
        {
            if(enemyCenter.x < position.x)
            {
                Move(Vector3.left);
            }

            if(enemyCenter.x > position.x)
            {
                Move(Vector3.right);
            }
        }
    }

    public void OnDeathAnimFinished()
    {
        Destroy(gameObject);
    }
}