using UnityEngine;

[CreateAssetMenu(fileName = "UnitStats", menuName = "UnitStats", order = 1)]
public class UnitStats : ScriptableObject
{
    public float health;
    public float attack;
    public float defense;
    public float speed = 0.1f;
    public float attackRange;
    public float postAttackDelay;
    public float maxAttackCooldown;
}