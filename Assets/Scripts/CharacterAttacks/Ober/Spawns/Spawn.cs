using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawn : MonoBehaviour
{
    public SpawnConfig Config { get; private set; }

    public void Initialize(SpawnConfig config) => Config = config;

    public void PerformAction(string action)
    {
        Debug.Log($"{Config.type} performs action: {action}");
    }
}
