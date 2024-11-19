using System;
using UnityEngine;

public class ArcherArrow : MonoBehaviour
{
    public float speed;

    // TODO encapsulate this
    public Army EnemyArmy;

    // TODO put these in a SO
    [NonSerialized]
    public float Attack;

    [NonSerialized]
    public Vector3 Target;

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
        movement = direction * speed;
        CachedTransform.forward = direction;
    }

    public void Update()
    {
        transform.position += movement;

        Vector3 position = CachedTransform.position;

        foreach(UnitBase unit in EnemyArmy.Units)
        {
            float dist = Vector3.Distance(unit.CachedTransform.position, position);

            if(dist < speed)
            {
                unit.Hit(gameObject);
                Destroy(gameObject);
                return;
            }
        }

        if(Vector3.Distance(position, Target) < speed)
        {
            Destroy(gameObject);
        }
    }
}