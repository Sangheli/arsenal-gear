using System.Collections.Generic;
using UnityEngine;

public class SpawnPositionManager
{
    private const int MaxSize = 256;
    private readonly bool[] _occupied = new bool[MaxSize];

    public bool GetPosition(UnitPosConfig posConfig, out Vector3 spawnPosition, out int zPosition)
    {
        zPosition = 0;
        spawnPosition = Vector3.zero;

        int minZ = Mathf.RoundToInt(G.I.minZ);
        int maxZ = Mathf.RoundToInt(G.I.maxZ);
        minZ = Mathf.Clamp(minZ, 0, MaxSize - 1);
        maxZ = Mathf.Clamp(maxZ, 0, MaxSize - 1);

        var overrideZ = posConfig.GetZPos() ?? Mathf.Round(Random.Range(G.I.minZ, G.I.maxZ));
        var overrideY = posConfig.GetYPos() ?? G.I.posY;
        
        if (posConfig.ignoreSpawnPositionManager)
        {
            zPosition = Mathf.RoundToInt(overrideZ);
            spawnPosition = ScreenSpawnHelper.GetRandomScreenPosition(overrideY, overrideZ);
            return true;
        }

        // Если задан overrideZ – пробуем именно эту позицию
        if (posConfig.GetZPos() != null)
        {
            zPosition = Mathf.RoundToInt(overrideZ);
            if (zPosition >= minZ && zPosition <= maxZ && !_occupied[zPosition])
            {
                spawnPosition = ScreenSpawnHelper.GetRandomScreenPosition(overrideY ,overrideZ);
                _occupied[zPosition] = true;
                return true;
            }
            return false; // overrideZ занят или вне диапазона
        }

        // Поиск первой свободной позиции
        for (int z = minZ; z <= maxZ; z++)
        {
            if (!_occupied[z])
            {
                zPosition = z;
                spawnPosition = ScreenSpawnHelper.GetRandomScreenPosition(overrideY, z);
                _occupied[z] = true;
                return true;
            }
        }
        
        // Все занято
        return false;
    }

    public void Release(IZPosHandler enemy)
    {
        if (enemy.ZPos is >= 0 and < MaxSize)
            _occupied[enemy.ZPos] = false;
    }

    public void Clear()
    {
        for (int i = 0; i < MaxSize; i++)
            _occupied[i] = false;
    }
}
