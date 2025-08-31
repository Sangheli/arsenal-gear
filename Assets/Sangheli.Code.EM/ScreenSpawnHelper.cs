using UnityEngine;

public static class ScreenSpawnHelper
{
    private static readonly Vector2[] Offsets =
    {
        new(-1, 0), // Left
        new(1, 0),  // Right
        new(0, -1), // Bottom
        new(0, 1)   // Top
    };

    public static Vector3 GetRandomScreenPosition(float yPos, float zPos)
    {
        Vector2 min = new Vector2(-G.I.MaxX, -G.I.MaxY);
        Vector2 max = new Vector2(G.I.MaxX, G.I.MaxY);
        
        return GetRandomScreenPosition(min, max, yPos, zPos);
    }
    
    public static Vector2 GetRandomScreenPosition(Camera camera,float yPos, float zPos)
    {
        Vector2 min = camera.ViewportToWorldPoint(new Vector2(0f, 0f));
        Vector2 max = camera.ViewportToWorldPoint(new Vector2(1f, 1f));
        
        return GetRandomScreenPosition(min, max,yPos, zPos);
    }
    
    public static Vector3 GetRandomScreenPosition(Vector2 min, Vector2 max,float yPos, float zPos)
    {
        var side = Random.Range(0, 4);
        
        var pos = new Vector2(
            Random.Range(min.x, max.x),
            Random.Range(min.y, max.y)
        );
        
        var offset = Offsets[side];
        var dx = offset.x != 0 ? Random.Range(0.1f, 0.3f) * offset.x : 0f;
        var dy = offset.y != 0 ? Random.Range(0.1f, 0.3f) * offset.y : 0f;
        
        return new Vector3
        (
            offset.x != 0 ? (offset.x < 0 ? min.x + dx : max.x + dx) : pos.x,
            yPos,
            zPos
        );
    }
    
    public static Vector2 GetMovementDirection(Vector2 spawnPosition)
    {
        Vector2 min = new Vector2(-G.I.MaxX, -G.I.MaxY);
        Vector2 max = new Vector2(G.I.MaxX, G.I.MaxY);
        var result = GetPos(spawnPosition, min, max);

        result.y = 0;
        
        return result;
    }

    public static Vector2 GetMovementDirection(Camera camera, Vector2 spawnPosition)
    {
        Vector2 min = camera.ViewportToWorldPoint(new Vector2(0f, 0f));
        Vector2 max = camera.ViewportToWorldPoint(new Vector2(1f, 1f));
        return GetPos(spawnPosition, min, max);
    }

    private static Vector2 GetPos(Vector2 spawnPosition, Vector2 min, Vector2 max)
    {
        var value = (min + max) / 2f - spawnPosition;
        
        if (spawnPosition.x < min.x)
            value = new Vector2(max.x + Random.Range(1f, 3f), Random.Range(min.y, max.y)) - spawnPosition;
        if (spawnPosition.x > max.x)
            value = new Vector2(min.x - Random.Range(1f, 3f), Random.Range(min.y, max.y)) - spawnPosition;
        if (spawnPosition.y < min.y)
            value = new Vector2(Random.Range(min.x, max.x), max.y + Random.Range(1f, 3f)) - spawnPosition;
        if (spawnPosition.y > max.y)
            value =  new Vector2(Random.Range(min.x, max.x), min.y - Random.Range(1f, 3f)) - spawnPosition;

        return value.normalized;
    }
} 