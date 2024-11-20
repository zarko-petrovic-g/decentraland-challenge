using System;
using UnityEngine;

public class ArcherArrow : MonoBehaviour
{
    [NonSerialized]
    public float Speed;

    [NonSerialized]
    public float Attack;

    [NonSerialized]
    public Vector3 Target;

    public Army EnemyArmy;

    public Transform CachedTransform { get; private set; }
    private Vector3 direction;
    private Vector3 movement;
    
    private new Renderer renderer;
    
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
        transform.position += movement;

        Vector3 position = CachedTransform.position;

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