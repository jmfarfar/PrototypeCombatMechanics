using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="SpawnConfig", menuName ="Spawn Config")]
public class SpawnConfig : ScriptableObject
{
    public GameObject spawnPrefab;
    public string type;
    public int health;
    public int damage;
    public float attackRange;
}


