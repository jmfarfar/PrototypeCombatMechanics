using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnFactory : ISpawnFactory
{
    public Spawn Create(SpawnConfig config)
    {
        GameObject spawn = Object.Instantiate(config.spawnPrefab);
        Spawn spawnComponent = spawn.GetComponent<Spawn>();
        spawnComponent.Initialize(config);
        return spawnComponent;
    }
}
