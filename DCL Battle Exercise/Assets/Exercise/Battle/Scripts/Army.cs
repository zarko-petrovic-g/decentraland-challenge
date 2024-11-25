using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Army
{
    private readonly List<UnitBase> units = new List<UnitBase>();

    private UnitsGrid unitsGrid;
    public IEnumerable<UnitBase> Units => units;

    public int UnitCount => units.Count;
    public Vector3 Center { get; private set; }

    public void InstantiateUnits(IArmyModel model, Bounds bounds, Color color, Army enemyArmy, UnitPrefab[] unitPrefabs,
        Battle battle)
    {
        unitsGrid = new UnitsGrid(battle.BattlefieldSize, battle.PartitionSize, model.TotalUnits);

        int unitsInstantiated = 0;

        foreach(UnitCount unitCount in model.UnitCounts)
        {
            UnitPrefab unitPrefab = unitPrefabs.First(prefab => prefab.type == unitCount.type);

            for(int i = 0; i < unitCount.count; i++)
            {
                InstantiateUnit(unitPrefab.prefab, bounds, model.Strategy, color, enemyArmy, battle,
                    unitsInstantiated++);
            }
        }

        Center = Utils.GetCenter(units);
    }

    private void InstantiateUnit(UnitBase original, Bounds bounds, ArmyStrategy strategy, Color color, Army enemyArmy,
        Battle battle, int index)
    {
        UnitBase unit = Object.Instantiate(original, Utils.GetRandomPosInBounds(bounds), Quaternion.identity);

        unit.EnemyArmy = enemyArmy;
        unit.Color = color;
        unit.Battle = battle;
        unit.ArmyStrategy = strategy;
        unit.Index = index;
        unit.OnDeath += Remove;
        unit.OnMove += unitsGrid.OnUnitMoved;

        units.Add(unit);
        unitsGrid.Add(unit);
    }

    public void Update()
    {
        Center = Utils.GetCenter(units);
    }

    private void Remove(UnitBase unit)
    {
        // not unsubscribing from events because it allocates memory
        // https://stackoverflow.com/questions/29587567/high-memory-allocations-when-unregistering-delegates-from-event-in-c-sharp
        // it would be natural to unsubscribe but since our units don't update when dead,
        // and we remove them from the collections, it's not necessary

        // unit.OnDeath -= Remove;
        // unit.OnMove -= unitsGrid.OnUnitMoved;

        bool removed = units.Remove(unit);

        if(!removed)
        {
            Debug.LogError("Unit not found in the army", unit);
        }

        unitsGrid.Remove(unit);
    }

    public bool GetClosestUnit(Vector3 source, out float minDistance, out UnitBase nearestEnemy)
    {
        return unitsGrid.GetClosest(source, out nearestEnemy, out minDistance);
    }

    public void EvadeOtherUnits(UnitBase unit, float minUnitDistance)
    {
        unitsGrid.EvadeOtherUnits(unit, minUnitDistance, unit.EnemyArmy != this);
    }

    /// <summary>
    ///     Finds a unit within a given range from a given position.
    /// </summary>
    public bool FindUnit(Vector3 position, float radius, out UnitBase unit)
    {
        return unitsGrid.FindUnit(position, radius, out unit);
    }

    /// <summary>
    ///     Finds the unit with the least health within a given range from a given position.
    /// </summary>
    public bool FindMinHealthUnit(Vector3 position, float radius, out UnitBase unit)
    {
        return unitsGrid.FindMinHealthUnit(position, radius, out unit);
    }

    public int GetUnits(Vector3 position, float damageRadius, UnitBase[] hits)
    {
        return unitsGrid.GetUnits(position, damageRadius, hits);
    }
}