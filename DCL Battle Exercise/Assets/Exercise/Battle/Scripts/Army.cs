using System.Collections.Generic;
using UnityEngine;

public class Army
{
    private readonly List<UnitBase> units = new List<UnitBase>();
    public IEnumerable<UnitBase> Units => units;

    public int UnitCount => units.Count;
    public Vector3 Center { get; private set; }
    
    private UnitsGrid unitsGrid;

    public void InstantiateUnits(IArmyModel model, Bounds bounds, Warrior warriorPrefab, Archer archerPrefab,
        Color color, Army enemyArmy, Battle battle)
    {
        unitsGrid = new UnitsGrid(battle.BattlefieldSize, battle.PartitionSize);
        
        for(int i = 0; i < model.warriors; i++)
        {
            InstantiateUnit(warriorPrefab, bounds, model.strategy, color, enemyArmy, battle);
        }

        for(int i = 0; i < model.archers; i++) 
        {
            InstantiateUnit(archerPrefab, bounds, model.strategy, color, enemyArmy, battle);
        }

        Center = Utils.GetCenter(Units);
    }

    private void InstantiateUnit(UnitBase original, Bounds bounds, ArmyStrategy strategy, Color color, Army enemyArmy,
        Battle battle)
    {
        UnitBase unit = Object.Instantiate(original, Utils.GetRandomPosInBounds(bounds), Quaternion.identity);

        unit.EnemyArmy = enemyArmy;
        unit.Color = color;
        unit.Battle = battle;
        unit.ArmyStrategy = strategy;
        unit.OnDeath += Remove;
        unit.OnMove += unitsGrid.OnUnitMoved;
        
        units.Add(unit);
        unitsGrid.Add(unit);
    }

    public void Update()
    {
        Center = Utils.GetCenter(Units);
    }

    private void Remove(UnitBase unit)
    {
        unit.OnDeath -= Remove;
        unit.OnMove -= unitsGrid.OnUnitMoved;
        bool removed = units.Remove(unit);

        if(!removed)
        {
            Debug.LogError("Unit not found in the army", unit);
        }
        unitsGrid.Remove(unit);
    }
    
    public bool GetNearestUnit(Vector3 source, out float minDistance, out UnitBase nearestEnemy)
    {
        return unitsGrid.GetClosest(source, out nearestEnemy, out minDistance);
    }
}