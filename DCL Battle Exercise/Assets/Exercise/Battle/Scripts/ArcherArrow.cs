using System;
using UnityEngine;

public class ArcherArrow : MonoBehaviour
{
    public float speed;

    // TODO encapsulate this and convert to enemy army since it only uses it to check for enemies (or remove altogether if a target property is used)
    public Army army;

    [NonSerialized]
    public float attack;

    [NonSerialized]
    public Vector3 target;

    public void Update()
    {
        Vector3 direction = (target - transform.position).normalized;
        transform.position += direction * speed;
        transform.forward = direction;

        // TODO just check the target enemy instead of all enemies
        foreach(UnitBase a in army.EnemyArmy.Units)
        {
            float dist = Vector3.Distance(a.transform.position, transform.position);

            if(dist < speed)
            {
                var unit = a.GetComponent<UnitBase>();
                unit.Hit(gameObject);
                Destroy(gameObject);
                return;
            }
        }

        if(Vector3.Distance(transform.position, target) < speed)
        {
            Destroy(gameObject);
        }
    }
}