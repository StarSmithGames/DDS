using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class RandomExtensions
{
    public static int RandomNumBtw(this Vector2Int minMax)
    {
        return Random.Range(minMax.x, minMax.y);
    }
    public static float RandomNumBtw(this Vector2 minMax)
    {
        return Random.Range(minMax.x, minMax.y);
    }

    public static Vector3 RandomPointInAnnulus(Vector3 origin, float minRadius, float maxRadius)
    {
        Vector3 direction = GetRandomPointInCircleXZNoZero();//(Random.insideUnitCircle * origin).normalized;

        float distance = Random.Range(minRadius, maxRadius);

        Vector3 result = origin + direction * distance;

        return result;
    }

    public static Vector3 GetRandomPointInCircleXZ() => new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f));
    public static Vector3 GetRandomPointInCircleXZNoZero()
    {
        Vector3 result = GetRandomPointInCircleXZ();

        if (result.x == 0 && result.y == 0)
            return GetRandomPointInCircleXZNoZero();

        return result;
    }
}