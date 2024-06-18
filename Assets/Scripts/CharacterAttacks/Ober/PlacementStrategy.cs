using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="DefaultPlacer", menuName ="Placements/DefaultPlacer")]
public class PlacementStrategy : ScriptableObject
{
    public virtual Vector3 SetPosition(Vector3 origin) => origin;
}
