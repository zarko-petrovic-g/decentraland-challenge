using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
///     ScriptableObject containing the data of an army
///     for simplicity's sake the use-case of updating the SO manually has been discarded, and
///     therefore the usage of ReadOnlyAttribute
/// </summary>
[CreateAssetMenu(menuName = "ArmyModel", fileName = "ArmyModel", order = 0)]
public class ArmyModelSO : ScriptableObject, IArmyModel
{
    [ReadOnly, SerializeField] // ReadOnly attribute here bugs the inspector for some reason
    private UnitCount[] unitCounts = Array.Empty<UnitCount>();

    [ReadOnly, SerializeField]
    private ArmyStrategy strategyValue = ArmyStrategy.Basic;

    public int GetUnitCount(UnitType unitType)
    {
        UnitCount unitCount = unitCounts.FirstOrDefault(unitCount => unitCount.type == unitType);
        return unitCount?.count ?? 0;
    }

    public void SetUnitCount(UnitType unitType, int count)
    {
        UnitCount unitCount = unitCounts.FirstOrDefault(unitCount => unitCount.type == unitType);

        if(unitCount != null)
        {
            unitCount.count = count;
        }
        else
        {
            var newUnitCounts = new UnitCount[unitCounts.Length + 1];

            for(int i = 0; i < unitCounts.Length; i++)
            {
                newUnitCounts[i] = unitCounts[i];
            }

            unitCount = new UnitCount(unitType, count);
            newUnitCounts[unitCounts.Length] = unitCount;
            unitCounts = newUnitCounts;
        }
    }

    public IEnumerable<UnitCount> UnitCounts => unitCounts;
    public ArmyStrategy Strategy
    {
        get => strategyValue;
        set => strategyValue = value;
    }
    public int TotalUnits => unitCounts.Sum(unitCount => unitCount.count);
}