public interface IArmyModel

{
    int Warriors { get; set; }
    int Archers { get; set; }
    int Cannons { get; set; }
    ArmyStrategy Strategy { get; set; }
    int TotalUnits { get; }
}

public enum ArmyStrategy
{
    Basic = 0,
    Defensive = 1,
    Bloodthirsty = 2,
    Cowardly = 3
}