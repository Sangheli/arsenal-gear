using System;
using UnityEngine;

[Serializable]
public class EnemyDataPos
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

    [Space] 
    public bool useXPos;
    public float posX;
    
    public float? GetZPos() => useCustomZPos ? customZPos : null;
    public float? GetYPos() => useRanomYPos ? Mathf.Round(UnityEngine.Random.Range(minY, maxY)) : null;
    public float? GetXPos() => useXPos ? posX : null;
}