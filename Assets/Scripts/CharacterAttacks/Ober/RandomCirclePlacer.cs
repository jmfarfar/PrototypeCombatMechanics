using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="RandomCircle", menuName ="Placements/CircleAnnulus")]
public class RandomCirclePlacer : PlacementStrategy
{
    public float minDistance;
    public float maxDistance = 10f;

    public override Vector3 SetPosition(Vector3 origin)
    {
        return origin.RandomPointInAnnulus(minDistance, maxDistance);
    }
}
