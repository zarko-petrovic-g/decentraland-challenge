using UnityEngine;

public abstract class Projectile : MonoBehaviour, IPoolable
{
    protected float attack;

    protected Vector3 direction;

    protected Army enemyArmy;

    private Material material;
    protected Vector3 movement;

    protected Pool pool;

    private new Renderer renderer;

    protected float speed;

    protected Vector3 target;

    public Transform CachedTransform { get; private set; }

    public Color Color
    {
        set => material.color = value;
        get => material.color;
    }

    protected virtual void Awake()
    {
        CachedTransform = transform;
        renderer = GetComponent<Renderer>();
        material = renderer.material;
    }

    public virtual void AcquireFromPool()
    {
        enabled = true;
        renderer.enabled = true;
    }

    public virtual void ReturnToPool()
    {
        enabled = false;
        renderer.enabled = false;
    }

    public abstract PoolableCategory Category { get; }

    protected void Init(Vector3 position, Vector3 target, float attackDamage, Army enemyArmy, float speed,
        Color color, Pool pool)
    {
        CachedTransform.position = position;
        this.target = target;
        attack = attackDamage;
        this.enemyArmy = enemyArmy;
        this.speed = speed;
        Color = color;
        this.pool = pool;
        direction = (this.target - CachedTransform.position).normalized;
        CachedTransform.forward = direction;
        movement = direction * this.speed;
    }
}