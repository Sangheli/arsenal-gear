using System;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public class EnemyDataPos
{
    [Space] 
    public bool useCustomZPos;
    [FormerlySerializedAs("customZPos")] public float customZPosMin;
    public int customZPosMax;

    [Space]
    public bool ignoreSpawnPositionManager;

    [Space] 
    public bool useRanomYPos;
    public float minY;
    public float maxY;

    [Space] 
    public bool useXPos;
    public float posX;
    
    public int? GetZPos() => useCustomZPos ?  UnityEngine.Random.Range((int)customZPosMin, customZPosMax+1) : null;
    public float? GetYPos() => useRanomYPos ? Mathf.Round(UnityEngine.Random.Range(minY, maxY)) : null;
    public float? GetXPos() => useXPos ? posX : null;
}