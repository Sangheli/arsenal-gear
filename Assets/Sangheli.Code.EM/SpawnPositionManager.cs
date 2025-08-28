using System.Collections.Generic;
using UnityEngine;

public class SpawnPositionManager
{
    // Track occupied Z positions
    private readonly HashSet<int> _occupiedZPositions = new();

    public bool GetPosition(out Vector3 spawnPosition, out int zPosition)
    {
        int maxAttempts = 50; // Limit attempts to prevent infinite loop
        int attempts = 0;
        zPosition = 0;
        bool positionFound = false;
        
        do
        {
            spawnPosition = ScreenSpawnHelper.GetRandomScreenPosition();
            zPosition = Mathf.RoundToInt(spawnPosition.z);
            
            // Check if this Z position is available
            if (!_occupiedZPositions.Contains(zPosition))
            {
                positionFound = true;
                break;
            }
            
            attempts++;
        } while (attempts < maxAttempts);
        
        // If we couldn't find a free position, don't spawn an enemy
        if (!positionFound)
        {
            // All Z positions are occupied, skip spawning
            return false;
        }
        
        // Mark this Z position as occupied
        _occupiedZPositions.Add(zPosition);
        return true;
    }

    public void AddCallback(IZPosHandler enemy) => enemy.OnDeathZPosHolder += () => _occupiedZPositions.Remove(enemy.ZPos);
    public void Clear() => _occupiedZPositions.Clear();
}