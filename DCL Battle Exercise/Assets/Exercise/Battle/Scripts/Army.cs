using System.Collections.Generic;
using UnityEngine;

public class Army
{
    private readonly List<UnitBase> units = new List<UnitBase>();
    public IEnumerable<UnitBase> Units => units;

    public int UnitCount => units.Count;
    public Vector3 Center { get; private set; }

    public void InstantiateUnits(IArmyModel model, Bounds bounds, Warrior warriorPrefab, Archer archerPrefab,
        Color color, Army enemyArmy, Battle battle)
    {
        for(int i = 0; i < model.warriors; i++)
        {
            InstantiateUnit(archerPrefab, bounds, model.strategy, color, enemyArmy, battle);
        }

        for(int i = 0; i < model.archers; i++)
        {
            InstantiateUnit(warriorPrefab, bounds, model.strategy, color, enemyArmy, battle);
        }

        Center = Utils.GetCenter(Units);
    }

    private void InstantiateUnit(UnitBase original, Bounds bounds, ArmyStrategy strategy, Color color, Army enemyArmy,
        Battle battle)
    {
        UnitBase unit = Object.Instantiate(original, Utils.GetRandomPosInBounds(bounds), Quaternion.identity);

        unit.Army = this;
        unit.EnemyArmy = enemyArmy;
        unit.Color = color;
        unit.Battle = battle;
        unit.ArmyStrategy = strategy;

        units.Add(unit);
    }

    public void Update()
    {
        Center = Utils.GetCenter(Units);
    }

    public void Remove(UnitBase unit)
    {
        units.Remove(unit);
    }
}