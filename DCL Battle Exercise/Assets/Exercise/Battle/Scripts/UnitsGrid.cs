using System;
using System.Collections.Generic;
using UnityEngine;

public class UnitsGrid
{
    private static int idCounter;
    private readonly float cellSize;
    private readonly Queue<(int, int)> cellsToEvade = new Queue<(int, int)>();

    private readonly bool[,] evadedCells;
    private readonly bool[] evadedUnits;
    private readonly List<UnitBase>[,] grid;
    private readonly int gridSize;
    private readonly float halfBattlefieldSize;
    private readonly int id = idCounter++;
    private float battlefieldSize;

    public UnitsGrid(float battlefieldSize, float cellSize, int unitCount)
    {
        gridSize = Mathf.CeilToInt(battlefieldSize / cellSize);
        this.cellSize = cellSize;
        this.battlefieldSize = battlefieldSize;
        halfBattlefieldSize = battlefieldSize / 2f;
        grid = new List<UnitBase>[gridSize, gridSize];

        for(int i = 0; i < gridSize; i++)
        {
            for(int j = 0; j < gridSize; j++)
            {
                grid[i, j] = new List<UnitBase>(unitCount);
            }
        }

        evadedUnits = new bool[unitCount];
        evadedCells = new bool[gridSize, gridSize];

        // Debug.Log("Created grid with size " + gridSize + "x" + gridSize + " and cell size " + cellSize + ", battlefield size " + battlefieldSize + " ["+id+"]");
    }

    public void Add(UnitBase unit)
    {
        Vector3 position = unit.CachedTransform.position;
        (int x, int z) = GetGridIndices(position);
        grid[x, z].Add(unit);

        // Debug.Log("Added unit ta pos " + position + " to cell [" + x + "," + z + "]" + " " + unit.GetHashCode() + " ["+id+"]", unit);
    }

    public bool Remove(UnitBase unit)
    {
        Vector3 position = unit.CachedTransform.position;
        (int x, int z) = GetGridIndices(position);
        bool removed = grid[x, z].Remove(unit);

        if(!removed)
        {
            Debug.LogError(
                $"Unit not found in the expected cell [{x},{z}]" + " " + unit.GetHashCode() + " [" + id + "]", unit);
        }

        // Debug.Log("Removed unit from cell [" + x + "," + z + "]" + " " + unit.GetHashCode() + " ["+id+"]", unit);
        return removed;
    }

    public void OnUnitMoved(UnitBase unit, Vector3 oldPosition)
    {
        (int oldX, int oldZ) = GetGridIndices(oldPosition);
        (int x, int z) = GetGridIndices(unit.CachedTransform.position);

        if(oldX == x && oldZ == z)
        {
            return;
        }

        bool removed = grid[oldX, oldZ].Remove(unit);

        if(!removed)
        {
            Debug.LogError(
                $"Unit not found in the expected cell [{oldX},{oldZ}]" + " " + unit.GetHashCode() + " [" + id + "]",
                unit);
        }

        try
        {
            grid[x, z].Add(unit);
        }
        catch(Exception e)
        {
            Debug.LogError(
                "Couldn't add unit to the grid " + e.GetType() + " " + e.Message + " (" + x + "," + z + ") " +
                unit.CachedTransform.position, unit);
        }

        // Debug.Log("Moved unit from cell [" + oldX + "," + oldZ + "] to [" + x + "," + z + "]"  + " " + unit.GetHashCode() + " ["+id+"]", unit);
    }

    private (int, int) GetGridIndices(Vector3 position)
    {
        int x = Mathf.FloorToInt((position.x + halfBattlefieldSize) / cellSize);
        int z = Mathf.FloorToInt((position.z + halfBattlefieldSize) / cellSize);
        return (x, z);
    }

    private float GetMinEdgeDistance(Vector3 position, int cellX, int cellZ, int radiusCells)
    {
        // square edges
        float edgeLeft = Mathf.Max(0f, cellX - radiusCells) * cellSize - halfBattlefieldSize;
        float edgeRight = Mathf.Min(gridSize, cellX + radiusCells + 1) * cellSize - halfBattlefieldSize;
        float edgeBottom = Mathf.Max(0f, cellZ - radiusCells) * cellSize - halfBattlefieldSize;
        float edgeTop = Mathf.Min(gridSize, cellZ + radiusCells + 1) * cellSize - halfBattlefieldSize;

        // distance to edges
        float distanceLeft = position.x - edgeLeft;
        float distanceRight = edgeRight - position.x;
        float distanceBottom = position.z - edgeBottom;
        float distanceTop = edgeTop - position.z;

        // avoiding the params version of Mathf.Min to avoid allocating memory
        float minEdgeDistance = Mathf.Min(
            Mathf.Min(distanceLeft, distanceRight),
            Mathf.Min(distanceBottom, distanceTop)
        );

        return minEdgeDistance;
    }

    public bool GetClosest(Vector3 position, out UnitBase closest, out float distance)
    {
        // Debug.Log("Finding closest unit to " + position + " ["+id+"]");

        (int x, int z) = GetGridIndices(position);
        closest = null;
        distance = float.MaxValue;
        bool candidateFound = false;
        int radiusCells = 0;

        while(radiusCells < gridSize)
        {
            if(closest == null)
            {
                candidateFound = GetClosest(position, x, z, radiusCells, 1, out closest, out distance);
            }

            if(candidateFound)
            {
                // we have a candidate from the current square of cells defined by radius
                // but there could be a closer unit in the cells outside the current square
                // i.e. if this one was far diagonally but there's a close one horizontally or vertically
                // now we want to check that

                float minEdgeDistance = GetMinEdgeDistance(position, x, z, radiusCells);

                if(distance <= minEdgeDistance)
                {
                    // Debug.Log("No need to enlarge" + " ["+id+"]");

                    // if candidate is closer than all edges of the current square
                    // there can't be a closer unit outside the current square
                    break;
                }

                int checkRadiusCells = Mathf.CeilToInt((distance - minEdgeDistance) / cellSize);

                if(checkRadiusCells <= radiusCells)
                {
                    // Debug.Log("Enlarge radius is the same" + " ["+id+"]");
                    break;
                }

                UnitBase oldCandidate = closest;

                candidateFound = GetClosest(position, x, z, checkRadiusCells, checkRadiusCells - radiusCells,
                    out closest, out distance);

                if(candidateFound && closest == oldCandidate)
                {
                    // Debug.Log("No new candidate in larger radius" + " ["+id+"]");
                    break;
                }

                // we DID find a closer unit in the cells outside the current square
                // this is a new candidate, so we need to check the next square of cells
                // in the next iteration
                radiusCells = checkRadiusCells;
            }
            else
            {
                radiusCells++;
            }
        }

        // Debug.Log("Closest unit is " + (closest != null ? closest.GetHashCode().ToString() : "null") + " distance: " + distance + " ["+id+"]");

        return candidateFound;
    }

    private bool GetClosest(Vector3 position, int originX, int originZ, int radiusCells, int perimeter,
        out UnitBase closest, out float distance)
    {
        closest = null;
        distance = float.MaxValue;
        bool found = false;

        int left = Mathf.Max(0, originX - radiusCells);
        int right = Mathf.Min(gridSize - 1, originX + radiusCells);
        int bottom = Mathf.Max(0, originZ - radiusCells);
        int top = Mathf.Min(gridSize - 1, originZ + radiusCells);

        // Debug.Log("GetClosest: (" + originX + "," + originZ + ") p:" + perimeter + ", (" + left + "," + right + "," + bottom + "," + top + ") ["+id+"]");

        for(int x = left; x <= right; x++)
        {
            for(int z = bottom; z <= top; z++)
            {
                if(x >= left + perimeter && x <= right - perimeter && z >= bottom + perimeter && z <= top - perimeter)
                {
                    continue;
                }

                // Debug.Log("(" + x + "," + z + ") ["+id+"]");

                int count = grid[x, z].Count;

                for(int i = 0; i < count; i++)
                {
                    UnitBase unit = grid[x, z][i];
                    float currentDistance = Vector3.Distance(unit.CachedTransform.position, position);

                    if(currentDistance < distance)
                    {
                        distance = currentDistance;
                        closest = unit;
                        found = true;
                    }
                }
            }
        }

        // Debug.Log("GetClosest: " + (closest != null ? closest.GetHashCode().ToString() : "null") + " distance: " + distance + " radius: " + radiusCells + " ["+id+"]");
        return found;
    }

    public void EvadeOtherUnits(UnitBase unit, float minUnitDistance, bool isAllied)
    {
        for(int i = 0; i < evadedUnits.Length; i++)
        {
            evadedUnits[i] = false;
        }

        for(int i = 0; i < gridSize; i++)
        {
            for(int j = 0; j < gridSize; j++)
            {
                evadedCells[i, j] = false;
            }
        }

        cellsToEvade.Clear();

        Vector3 position = unit.CachedTransform.position;

        (int x, int z) = GetGridIndices(position);

        // Debug.Log("Evading other units, pos" + position + "(" + x + "," + z + ")" + " ["+id+"]" + " " + unit.GetHashCode(), unit);

        cellsToEvade.Enqueue((x, z));

        if(isAllied)
        {
            evadedUnits[unit.Index] = true;
        }

        while(cellsToEvade.Count > 0)
        {
            (int currentX, int currentZ) = cellsToEvade.Dequeue();
            List<UnitBase> units = grid[currentX, currentZ];
            int count = units.Count;

            bool moved = false;

            for(int i = 0; i < count; i++)
            {
                UnitBase otherUnit = units[i];

                if(evadedUnits[otherUnit.Index])
                {
                    // Debug.Log("Already evaded unit " + otherUnit.GetHashCode() + " ["+id+"]", otherUnit);
                    continue;
                }

                Vector3 toEvadePosition = otherUnit.CachedTransform.position;
                float distance = Vector3.Distance(position, toEvadePosition);

                if(distance < minUnitDistance)
                {
                    Vector3 toNearest = (toEvadePosition - position).normalized;
                    position -= toNearest * (minUnitDistance - distance);
                    moved = true;
                    // Debug.Log("Moving " + unit.GetHashCode() + " to evade " + otherUnit.GetHashCode() + " ["+id+"]" + "newPos: " + position, unit);
                }

                evadedUnits[otherUnit.Index] = true;
            }

            if(moved)
            {
                unit.SetPosition(position);
            }

            evadedCells[currentX, currentZ] = true;

            // cell edges
            float edgeLeft = currentX * cellSize - halfBattlefieldSize;
            float edgeRight = (currentX + 1) * cellSize - halfBattlefieldSize;
            float edgeBottom = currentZ * cellSize - halfBattlefieldSize;
            float edgeTop = (currentZ + 1) * cellSize - halfBattlefieldSize;

            // distance to edges
            float distanceLeft = position.x - edgeLeft;
            float distanceRight = edgeRight - position.x;
            float distanceBottom = position.z - edgeBottom;
            float distanceTop = edgeTop - position.z;

            bool evadeLeft = distanceLeft < minUnitDistance && currentX > 0;
            bool evadeRight = distanceRight < minUnitDistance && currentX < gridSize - 1;
            bool evadeTop = distanceTop < minUnitDistance && currentZ < gridSize - 1;
            bool evadeBottom = distanceBottom < minUnitDistance && currentZ > 0;

            if(evadeLeft)
            {
                EnqueueToEvade(currentX - 1, currentZ);

                if(evadeTop)
                {
                    EnqueueToEvade(currentX - 1, currentZ + 1);
                }

                if(evadeBottom)
                {
                    EnqueueToEvade(currentX - 1, currentZ - 1);
                }
            }

            if(evadeRight)
            {
                EnqueueToEvade(currentX + 1, currentZ);

                if(evadeTop)
                {
                    EnqueueToEvade(currentX + 1, currentZ + 1);
                }

                if(evadeBottom)
                {
                    EnqueueToEvade(currentX + 1, currentZ - 1);
                }
            }

            if(evadeBottom)
            {
                EnqueueToEvade(currentX, currentZ - 1);
            }

            if(evadeTop)
            {
                EnqueueToEvade(currentX, currentZ + 1);
            }
        }
    }

    private void EnqueueToEvade(int x, int z)
    {
        if(!evadedCells[x, z])
        {
            // Debug.Log("Enqueueing to evade " + x + "," + z + " ["+id+"]");
            cellsToEvade.Enqueue((x, z));
        }
    }

    /// <summary>
    ///     Finds a unit within a given range from a given position.
    /// </summary>
    public bool FindUnit(Vector3 position, float range, out UnitBase unit)
    {
        (int cellX, int cellZ) = GetGridIndices(position);
        float minDistance = GetMinEdgeDistance(position, cellX, cellZ, 0);

        int radiusCells = Mathf.CeilToInt((range - minDistance) / cellSize);

        int left = Mathf.Max(0, cellX - radiusCells);
        int right = Mathf.Min(gridSize - 1, cellX + radiusCells);
        int bottom = Mathf.Max(0, cellZ - radiusCells);
        int top = Mathf.Min(gridSize - 1, cellZ + radiusCells);

        for(int x = left; x <= right; x++)
        {
            for(int z = bottom; z <= top; z++)
            {
                List<UnitBase> units = grid[x, z];
                int count = units.Count;

                for(int i = 0; i < count; i++)
                {
                    unit = units[i];

                    if(Vector3.Distance(unit.CachedTransform.position, position) <= range)
                    {
                        return true;
                    }
                }
            }
        }

        unit = null;
        return false;
    }

    /// <summary>
    ///     Finds the unit with the least health within a given range from a given position.
    /// </summary>
    public bool FindMinHealthUnit(Vector3 position, float range, out UnitBase unit)
    {
        (int cellX, int cellZ) = GetGridIndices(position);
        float minDistance = GetMinEdgeDistance(position, cellX, cellZ, 0);

        int radiusCells = Mathf.CeilToInt((range - minDistance) / cellSize);

        int left = Mathf.Max(0, cellX - radiusCells);
        int right = Mathf.Min(gridSize - 1, cellX + radiusCells);
        int bottom = Mathf.Max(0, cellZ - radiusCells);
        int top = Mathf.Min(gridSize - 1, cellZ + radiusCells);

        float minHealth = float.MaxValue;
        unit = null;
        bool found = false;

        for(int x = left; x <= right; x++)
        {
            for(int z = bottom; z <= top; z++)
            {
                List<UnitBase> units = grid[x, z];
                int count = units.Count;

                for(int i = 0; i < count; i++)
                {
                    UnitBase currentUnit = units[i];
                    float distance = Vector3.Distance(currentUnit.CachedTransform.position, position);

                    if(distance <= range && currentUnit.CurrentHealth < minHealth)
                    {
                        minHealth = currentUnit.CurrentHealth;
                        unit = currentUnit;
                        found = true;
                    }
                }
            }
        }

        return found;
    }

    public int GetUnits(Vector3 position, float range, UnitBase[] hits)
    {
        (int cellX, int cellZ) = GetGridIndices(position);
        float minDistance = GetMinEdgeDistance(position, cellX, cellZ, 0);

        int radiusCells = Mathf.CeilToInt((range - minDistance) / cellSize);

        int left = Mathf.Max(0, cellX - radiusCells);
        int right = Mathf.Min(gridSize - 1, cellX + radiusCells);
        int bottom = Mathf.Max(0, cellZ - radiusCells);
        int top = Mathf.Min(gridSize - 1, cellZ + radiusCells);

        int found = 0;

        for(int x = left; x <= right; x++)
        {
            for(int z = bottom; z <= top; z++)
            {
                List<UnitBase> units = grid[x, z];
                int count = units.Count;

                for(int i = 0; i < count; i++)
                {
                    UnitBase currentUnit = units[i];
                    float distance = Vector3.Distance(currentUnit.CachedTransform.position, position);

                    if(distance <= range)
                    {
                        hits[found++] = currentUnit;
                    }
                }
            }
        }

        return found;
    }
}