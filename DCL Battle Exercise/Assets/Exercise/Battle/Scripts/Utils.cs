using System.Collections.Generic;
using UnityEngine;

public static class Utils
{
    public static Vector3 GetRandomPosInBounds(Bounds bounds)
    {
        Vector3 pos = Vector3.zero;
        pos.x = Random.Range(bounds.min.x, bounds.max.x);
        pos.z = Random.Range(bounds.min.z, bounds.max.z);
        return pos;
    }

    public static Vector3 GetCenter<T>(List<T> objects)
        where T : UnitBase
    {
        Vector3 result = Vector3.zero;
        int count = objects.Count;

        for(int i = 0; i < count; i++)
        {
            result += objects[i].CachedTransform.position;
        }

        result.x /= count;
        result.y /= count;
        result.z /= count;

        return result;
    }

    public static bool GetNearestEnemy(Vector3 source, IEnumerable<UnitBase> enemies, out float minDistance,
        out UnitBase nearestEnemy)
    {
        minDistance = float.MaxValue;
        nearestEnemy = null;
        bool enemyFound = false;

        foreach(UnitBase enemy in enemies)
        {
            float distance = Vector3.Distance(source, enemy.CachedTransform.position);

            if(distance < minDistance)
            {
                minDistance = distance;
                nearestEnemy = enemy;
                enemyFound = true;
            }
        }

        return enemyFound;
    }
}