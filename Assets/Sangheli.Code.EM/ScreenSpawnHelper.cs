using UnityEngine;

public static class ScreenSpawnHelper
{
    public static Vector2 GetRandomScreenPosition(Camera camera)
    {
        Vector2 min = camera.ViewportToWorldPoint(new Vector2(0f, 0f));
        Vector2 max = camera.ViewportToWorldPoint(new Vector2(1f, 1f));
        
        int side = Random.Range(0, 4);
        float x, y;
        switch (side)
        {
            case 0:
                x = min.x - Random.Range(0.1f, 0.3f);
                y = Random.Range(min.y, max.y);
                break;
            case 1:
                x = max.x + Random.Range(0.1f, 0.3f);
                y = Random.Range(min.y, max.y);
                break;
            case 2:
                x = Random.Range(min.x, max.x);
                y = min.y - Random.Range(0.1f, 0.3f);
                break;
            case 3:
                x = Random.Range(min.x, max.x);
                y = max.y + Random.Range(0.1f, 0.3f);
                break;
            default:
                x = min.x - 2f;
                y = min.y - 2f;
                break;
        }
        return new Vector2(x, y);
    }

    public static Vector2 GetMovementDirection(Camera camera, Vector2 spawnPosition)
    {
        Vector2 min = camera.ViewportToWorldPoint(new Vector2(0f, 0f));
        Vector2 max = camera.ViewportToWorldPoint(new Vector2(1f, 1f));
        bool isLeft = spawnPosition.x < min.x;
        bool isRight = spawnPosition.x > max.x;
        bool isBottom = spawnPosition.y < min.y;
        bool isTop = spawnPosition.y > max.y;
        Vector2 targetPosition;
        if (isLeft)
        {
            targetPosition = new Vector2(max.x + Random.Range(1f, 3f), Random.Range(min.y, max.y));
        }
        else if (isRight)
        {
            targetPosition = new Vector2(min.x - Random.Range(1f, 3f), Random.Range(min.y, max.y));
        }
        else if (isBottom)
        {
            targetPosition = new Vector2(Random.Range(min.x, max.x), max.y + Random.Range(1f, 3f));
        }
        else if (isTop)
        {
            targetPosition = new Vector2(Random.Range(min.x, max.x), min.y - Random.Range(1f, 3f));
        }
        else
        {
            targetPosition = (min + max) / 2f;
        }
        return (targetPosition - spawnPosition).normalized;
    }
} 