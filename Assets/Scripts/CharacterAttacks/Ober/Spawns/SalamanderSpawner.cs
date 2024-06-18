using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SalamanderSpawner : MonoBehaviour
{
    public float maxSpawns = 10;
    public List<SpawnConfig> spawnConfigs;
    public PlacementStrategy placementStrategy;
    Spawn salamander;
    Transform enemy;

    ISpawnFactory factory = new SpawnFactory();
    // Start is called before the first frame update
    void Awake()
    {
        SpawnSalamanders();
    }

    void SpawnSalamanders()
    {
        for (int i = 0; i < maxSpawns; i++)
        {
            // send to a coroutine to spawn 1 by 1
            salamander = factory.Create(spawnConfigs[i % spawnConfigs.Count]);
            salamander.transform.position = placementStrategy.SetPosition(transform.position);

            salamander.GetComponent<Hitbox>().SetUpHitboxActions(
                () => { },
                () => { },
                (Collider other) => { }
                );
        }
    }

    
}
