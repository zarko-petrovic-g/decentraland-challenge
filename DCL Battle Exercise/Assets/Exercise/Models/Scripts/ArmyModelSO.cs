using UnityEngine;


/// <summary>
/// ScriptableObject containing the data of an army
/// for simplicity's sake the use-case of updating the SO manually has been discarded, and
/// therefore the usage of ReadOnlyAttribute
/// </summary>
[CreateAssetMenu(menuName = "Create ArmyModel", fileName = "ArmyModel", order = 0)]
public class ArmyModelSO : ScriptableObject, IArmyModel
{
    [ReadOnly, SerializeField] private int warriorsValue = 100;
    public int Warriors
    {
        get => warriorsValue;
        set => warriorsValue = value;
    }

    [ReadOnly, SerializeField] private int archersValue = 100;
    public int Archers
    {
        get => archersValue;
        set => archersValue = value;
    }

    [ReadOnly, SerializeField] private int cannonsValue = 100;
    public int Cannons
    {
        get => cannonsValue;
        set => cannonsValue = value;
    }
    
    [ReadOnly, SerializeField] private ArmyStrategy strategyValue = ArmyStrategy.Basic;
    public ArmyStrategy Strategy
    {
        get => strategyValue;
        set => strategyValue = value;
    }
    public int TotalUnits => archersValue + warriorsValue + cannonsValue;
}