using System;
using UnityEngine;

[Serializable]
public class UnitPosConfig
{
    [Space] 
    public bool useCustomZPos;
    public float customZPos;

    [Space]
    public bool ignoreSpawnPositionManager;

    [Space] 
    public bool useRanomYPos;
    public float minY;
    public float maxY;
    
    public float? GetZPos() => useCustomZPos ? customZPos : null;
    public float? GetYPos() => useRanomYPos ? Mathf.Round(UnityEngine.Random.Range(minY, maxY)) : null;
}