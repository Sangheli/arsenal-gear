using UnityEngine;

public class ClampPosition : MonoBehaviour
{
    [Header("Границы движения")]
    public Vector2 minBounds = new(-5f, -5f);
    public Vector2 maxBounds = new(5f, 5f);

    /// <summary>
    /// Устанавливает позицию с ограничениями.
    /// </summary>
    public void SetPosition(Vector3 targetPos)
    {
        // Ограничиваем X и Y
        float clampedX = Mathf.Clamp(targetPos.x, minBounds.x, maxBounds.x);
        float clampedY = Mathf.Clamp(targetPos.y, minBounds.y, maxBounds.y);

        // Z оставляем от targetPos
        var transform1 = transform;
        transform1.position = new Vector3(clampedX, clampedY, transform1.position.z);
    }
}