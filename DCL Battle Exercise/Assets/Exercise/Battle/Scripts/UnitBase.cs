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

    protected float attackCooldown;

    [NonSerialized]
    public Battle Battle;

    public Army EnemyArmy;
    protected bool hasAnimator;

    [NonSerialized]
    public int Index;

    private Vector3 lastPosition;

    protected new Renderer renderer;

    protected IUnitStrategy UnitStrategy;

    public float AttackCooldown => attackCooldown;

    /// <summary>
    ///     Cached transform component for performance reasons, avoiding calling transform.get_position
    ///     which has a native part and is slower.
    ///     Do not modify position directly, use <see cref="Move" /> or <see cref="SetPosition" /> method instead.
    /// </summary>
    public Transform CachedTransform { get; private set; }

    public GameObject CachedGameObject { get; private set; }

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
        CachedGameObject = gameObject;
        animator = GetComponentInChildren<Animator>();
        hasAnimator = animator != null;
        renderer = GetComponentInChildren<Renderer>();
        CurrentHealth = MaxHealth;

        // name += "_" + GetHashCode();
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
    public event Action<UnitBase, Vector3> OnMove;

    public abstract void Attack(UnitBase enemy);

    /// <summary>
    ///     Use for gameplay unit movement. If you want to teleport unit, use <see cref="SetPosition" /> instead.
    /// </summary>
    /// <param name="delta">Normalized direction of movement</param>
    public virtual void Move(Vector3 delta)
    {
        if(attackCooldown > MaxAttackCooldown - PostAttackDelay)
        {
            return;
        }

        Vector3 oldPosition = CachedTransform.position;
        CachedTransform.position += delta * Speed;
        OnMove?.Invoke(this, oldPosition);
    }

    /// <summary>
    ///     Used to teleport unit to a new position. Use <see cref="Move" /> for gameplay movement.
    /// </summary>
    /// <param name="position">New position</param>
    public void SetPosition(Vector3 position)
    {
        Vector3 oldPosition = CachedTransform.position;
        CachedTransform.position = position;
        OnMove?.Invoke(this, oldPosition);
    }

    public virtual void Hit(float damage, Vector3 attackerPosition)
    {
        CurrentHealth -= Mathf.Max(damage - Defense, 0);

        if(CurrentHealth < 0)
        {
            CachedTransform.forward = attackerPosition - CachedTransform.position;
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
        Destroy(CachedGameObject);
    }
}