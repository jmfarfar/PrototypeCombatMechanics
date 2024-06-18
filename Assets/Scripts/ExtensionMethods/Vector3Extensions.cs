using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Vector3Extensions
{
    /// <summary>
    /// Sets any xyz values of a Vector3
    /// </summary>
    public static Vector3 With(this Vector3 vector, float? x = null, float? y = null, float? z = null)
    {
        return new Vector3(x ?? vector.x, y ?? vector.y, z ?? vector.z);
    }

    /// <summary>
    /// Computes a random point in an annulus based on minimum and maximum radius values around
    /// a centrall Vector3 point(origin).
    /// </summary>
    /// <param name="origin">The center Vector3 point of the annulus.</param>
    /// <param name="midRadius">Minimum radius of the annulus.</param>
    /// <param name="maxRadius">Maximum radius of the annulus.</param>
    /// <returns>A random Vector3 point within the specified annulus.</returns>
    public static Vector3 RandomPointInAnnulus(this Vector3 origin, float minRadius, float maxRadius)
    {
        float angle = Random.value * Mathf.PI * 2f;
        Vector2 dir = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));

        float minRadius2 = minRadius * minRadius;
        float maxRadius2 = maxRadius * maxRadius;
        float distance = Mathf.Sqrt(Random.value * (maxRadius2 - minRadius2) + minRadius2);
        Vector3 position = Vector3.zero.With(x: dir.x, z: dir.y) * distance;
        return origin + position;
    }

}
