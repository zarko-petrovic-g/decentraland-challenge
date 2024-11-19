using System.Collections.Generic;
using UnityEngine;

public class Army
{
    public Army EnemyArmy { get; set; }

    // TODO consider returning IEnumerable instead, does it create garbage in a foreach loop?
    public List<UnitBase> Units { get; } = new List<UnitBase>();

    public int UnitCount => Units.Count;
    public Vector3 Center => Utils.GetCenter(Units);

    public void InstantiateUnits(IArmyModel model, Bounds bounds, Warrior warriorPrefab, Archer archerPrefab,
        Color color)
    {
        for(int i = 0; i < model.warriors; i++)
        {
            Warrior warrior = Object.Instantiate(warriorPrefab);
            warrior.transform.position = Utils.GetRandomPosInBounds(bounds);

            warrior.army = this;
            warrior.armyModel = model;
            warrior.GetComponentInChildren<Renderer>().material.color = color;

            Units.Add(warrior);
        }

        for(int i = 0; i < model.archers; i++)
        {
            Archer archer = Object.Instantiate(archerPrefab);
            archer.transform.position = Utils.GetRandomPosInBounds(bounds);

            archer.army = this;
            archer.armyModel = model;
            archer.GetComponentInChildren<Renderer>().material.color = color;

            Units.Add(archer);
        }
    }

    public void Remove(UnitBase unit)
    {
        Units.Remove(unit);
    }
}