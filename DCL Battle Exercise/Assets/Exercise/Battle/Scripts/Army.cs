using System.Collections.Generic;
using UnityEngine;

public class Army
{
    // TODO maybe an army should not know about its enemy, but the battle manager should know about both armies
    public Army EnemyArmy { get; set; }

    // TODO consider returning IEnumerable instead, does it create garbage in a foreach loop?
    public List<UnitBase> Units { get; } = new List<UnitBase>();

    public int UnitCount => Units.Count;
    public Vector3 Center { get; private set; }

    public void InstantiateUnits(IArmyModel model, Bounds bounds, Warrior warriorPrefab, Archer archerPrefab,
        Color color)
    {
        for(int i = 0; i < model.warriors; i++)
        {
            Warrior warrior = Object.Instantiate(warriorPrefab, Utils.GetRandomPosInBounds(bounds), Quaternion.identity);

            warrior.army = this;
            warrior.armyModel = model;
            warrior.Color = color;

            Units.Add(warrior);
        }

        for(int i = 0; i < model.archers; i++)
        {
            Archer archer = Object.Instantiate(archerPrefab);
            archer.transform.position = Utils.GetRandomPosInBounds(bounds);

            archer.army = this;
            archer.armyModel = model;
            archer.Color = color;

            Units.Add(archer);
        }

        Center = Utils.GetCenter(Units);
    }

    public void Update()
    {
        Center = Utils.GetCenter(Units);
    }

    public void Remove(UnitBase unit)
    {
        Units.Remove(unit);
    }
}