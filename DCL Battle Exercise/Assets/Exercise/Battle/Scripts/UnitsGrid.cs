using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class UnitsGrid
{
    private List<UnitBase>[,] grid;
    private int gridSize;
    private float cellSize;
    private float battlefieldSize;
    private float halfBattlefieldSize;
    
    private static int idCounter = 0;
    private int id = idCounter++;
    
    public UnitsGrid(float battlefieldSize, float cellSize)
    {
        gridSize = Mathf.CeilToInt(battlefieldSize / cellSize);
        this.cellSize = cellSize;
        this.battlefieldSize = battlefieldSize;
        halfBattlefieldSize = battlefieldSize / 2f;
        grid = new List<UnitBase>[gridSize, gridSize];
        for (int i=0; i<gridSize; i++)
        {
            for (int j=0; j<gridSize; j++)
            {
                grid[i, j] = new List<UnitBase>();
            }
        }

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
        if (!removed)
        {
            Debug.LogError($"Unit not found in the expected cell [{x},{z}]"  + " " + unit.GetHashCode() + " ["+id+"]", unit);
        }
        else
        {
            // Debug.Log("Removed unit from cell [" + x + "," + z + "]" + " " + unit.GetHashCode() + " ["+id+"]", unit);
        }
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
        
        if (!removed)
        {
            Debug.LogError($"Unit not found in the expected cell [{oldX},{oldZ}]"  + " " + unit.GetHashCode() + " ["+id+"]", unit);
        }
        
        grid[x, z].Add(unit);
        
        // Debug.Log("Moved unit from cell [" + oldX + "," + oldZ + "] to [" + x + "," + z + "]"  + " " + unit.GetHashCode() + " ["+id+"]", unit);
    }  
    
    private (int, int) GetGridIndices(Vector3 position)
    {
        int x = Mathf.FloorToInt((position.x + halfBattlefieldSize) / cellSize);
        int z = Mathf.FloorToInt((position.z + halfBattlefieldSize) / cellSize);
        return (x, z);
    }
    
    private float GetMinDistance(Vector3 position, int cellX, int cellZ, int radiusCells)
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
        
        float minEdgeDistance = Mathf.Min(distanceLeft, distanceRight, distanceBottom, distanceTop);
        
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
                candidateFound = GetClosest(position, x, z, radiusCells, out closest, out distance);
            }

            if(candidateFound)
            {
                // we have a candidate from the current square of cells defined by radius
                // but there could be a closer unit in the cells outside the current square
                // i.e. if this one was far diagonally but there's a close one horizontally or vertically
                // now we want to check that

                float minEdgeDistance = GetMinDistance(position, x, z, radiusCells);

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

                candidateFound = GetClosest(position, x, z, checkRadiusCells, out closest, out distance);

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

    private bool GetClosest(Vector3 position, int originX, int originZ, int radiusCells, out UnitBase closest, out float distance)
    {
        closest = null;
        distance = float.MaxValue;
        bool found = false;

        int left = Mathf.Max(0, originX-radiusCells);
        int right = Mathf.Min(gridSize-1, originX+radiusCells);
        int bottom = Mathf.Max(0, originZ-radiusCells);
        int top = Mathf.Min(gridSize-1, originZ+radiusCells);

        for (int x = left; x <= right; x++)
        {
            for (int z = bottom; z <= top; z++)
            {
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
}
