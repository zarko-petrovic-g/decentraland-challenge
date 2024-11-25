using System.Collections.Generic;

public interface IArmyModel
{
    int GetUnitCount(UnitType unitType);
    void SetUnitCount(UnitType unitType, int count);
    IEnumerable<UnitCount> UnitCounts { get; }
    ArmyStrategy Strategy { get; set; }
    int TotalUnits { get; }
}