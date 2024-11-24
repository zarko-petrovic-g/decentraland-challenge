using System;
using UnityEngine;

public class ArcherArrow : MonoBehaviour, IPoolable
{
    [NonSerialized]
    public float Attack;

    private Vector3 direction;

    public Army EnemyArmy;

    private Material material;
    private Vector3 movement;
    private Pool pool;

    private new Renderer renderer;

    [NonSerialized]
    public float Speed;

    [NonSerialized]
    public Vector3 Target;

    public Transform CachedTransform { get; private set; }

    public Color Color
    {
        set => material.color = value;
        get => material.color;
    }

    private void Awake()
    {
        CachedTransform = transform;
        renderer = GetComponent<Renderer>();
        material = renderer.material;
    }

    public void Update()
    {
        Vector3 position = CachedTransform.position;
        position += movement;
        CachedTransform.position = position;

        if(EnemyArmy.FindUnit(position, Speed, out UnitBase unit))
        {
            unit.Hit(Attack, position);
            pool.Return(this);
            return;
        }

        if(Vector3.Distance(position, Target) < Speed)
        {
            pool.Return(this);
        }
    }

    public void AcquireFromPool()
    {
        enabled = true;
        renderer.enabled = true;
    }

    public void ReturnToPool()
    {
        enabled = false;
        renderer.enabled = false;
    }

    public PoolableCategory Category => PoolableCategory.ArcherArrow;

    public void Init(Vector3 position, Vector3 target, float attackDamage, Army enemyArmy, float archerStatsArrowSpeed,
        Color color, Pool pool)
    {
        CachedTransform.position = position;
        Target = target;
        Attack = attackDamage;
        EnemyArmy = enemyArmy;
        Speed = archerStatsArrowSpeed;
        Color = color;
        this.pool = pool;
        direction = (Target - CachedTransform.position).normalized;
        movement = direction * Speed;
        CachedTransform.forward = direction;
    }
}