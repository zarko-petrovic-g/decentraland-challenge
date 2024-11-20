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

    public static Vector3 GetCenter<T>(IEnumerable<T> objects)
        where T : MonoBehaviour
    {
        Vector3 result = Vector3.zero;
        int count = 0;

        foreach(T o in objects)
        {
            result += o.transform.position;
            count++;
        }

        result.x /= count;
        result.y /= count;
        result.z /= count;

        return result;
    }

    // more performant compared to the IEnumerable version
    public static Vector3 GetCenter<T>(List<T> objects)
        where T : MonoBehaviour
    {
        Vector3 result = Vector3.zero;
        int count = objects.Count;

        for(int i = 0; i < count; i++)
        {
            result += objects[i].transform.position;
        }

        result.x /= count;
        result.y /= count;
        result.z /= count;

        return result;
    }

    public static bool GetNearestEnemy(Vector3 source, IEnumerable<UnitBase> enemies, out float minDistance,
        out UnitBase nearestEnemy)
    {
        // TODO convert to return bool if successful so the caller doesn't need to check a UnityObject for null

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