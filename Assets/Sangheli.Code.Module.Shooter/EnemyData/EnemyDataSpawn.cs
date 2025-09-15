using System;
using UnityEngine;

[Serializable]
public class EnemyDataSpawn
{
    [Space,Header("Spawn")]
    public bool spawnAtStart;
    public bool spawnAtStartHardcore;
    public bool useRandomSpawnRate;
    public int spawnRate1;
    public int spawnRate2;
    
    public int GetSpawnRate() => useRandomSpawnRate ? UnityEngine.Random.Range(spawnRate1, spawnRate2) : spawnRate1;
}