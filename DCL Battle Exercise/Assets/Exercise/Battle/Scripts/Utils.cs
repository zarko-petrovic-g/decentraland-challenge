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
        where T : MonoBehaviour
    {
        Vector3 result = Vector3.zero;

        foreach(T o in objects)
        {
            result += o.transform.position;
        }

        result.x /= objects.Count;
        result.y /= objects.Count;
        result.z /= objects.Count;

        return result;
    }

    public static float GetNearestEnemy(GameObject source, List<UnitBase> enemies, out UnitBase nearestEnemy)
    {
        // TODO convert to return bool if successful so the caller doesn't need to check a UnityObject for null

        float minDist = float.MaxValue;
        nearestEnemy = null;

        foreach(UnitBase enemy in enemies)
        {
            float dist = Vector3.Distance(source.transform.position, enemy.transform.position);

            if(dist < minDist)
            {
                minDist = dist;
                nearestEnemy = enemy;
            }
        }

        return minDist;
    }
}