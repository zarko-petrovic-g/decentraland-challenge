using System;
using UnityEngine;

public class ArcherArrow : MonoBehaviour
{
    [NonSerialized]
    public float Attack;

    private Vector3 direction;

    public Army EnemyArmy;
    private Vector3 movement;

    private new Renderer renderer;

    [NonSerialized]
    public float Speed;

    [NonSerialized]
    public Vector3 Target;

    public Transform CachedTransform { get; private set; }

    public Color Color
    {
        set => renderer.material.color = value;
        get => renderer.material.color;
    }

    private void Awake()
    {
        CachedTransform = transform;
        renderer = GetComponent<Renderer>();
    }

    private void Start()
    {
        direction = (Target - CachedTransform.position).normalized;
        movement = direction * Speed;
        CachedTransform.forward = direction;
    }

    public void Update()
    {
        CachedTransform.position += movement;

        Vector3 position = CachedTransform.position;

        // TODO use partitions
        foreach(UnitBase unit in EnemyArmy.Units)
        {
            float dist = Vector3.Distance(unit.CachedTransform.position, position);

            if(dist < Speed)
            {
                unit.Hit(Attack, position);
                Destroy(gameObject);
                return;
            }
        }

        if(Vector3.Distance(position, Target) < Speed)
        {
            Destroy(gameObject);
        }
    }
}