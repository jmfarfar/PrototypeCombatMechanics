using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISpawnFactory
{
    Spawn Create(SpawnConfig config);
}
