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
        where T : Component
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

    public static Vector3 GetCenter(List<GameObject> objects)
    {
        // TODO not getting the center, getting average position?

        Vector3 result = Vector3.zero;

        foreach(GameObject o in objects)
        {
            result += o.transform.position;
        }

        result.x /= objects.Count;
        result.y /= objects.Count;
        result.z /= objects.Count;

        return result;
    }

    public static float GetNearestObject(GameObject source, List<GameObject> objects, out GameObject nearestObject)
    {
        float minDist = float.MaxValue;
        nearestObject = null;

        foreach(GameObject obj in objects)
        {
            float dist = Vector3.Distance(source.transform.position, obj.transform.position);

            if(dist < minDist)
            {
                minDist = dist;
                nearestObject = obj;
            }
        }

        return minDist;
    }
}