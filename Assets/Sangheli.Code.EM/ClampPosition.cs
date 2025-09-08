using UnityEngine;

public class ClampPosition : MonoBehaviour
{
    [Header("Границы движения")]
    public Vector2 minBounds = new(-5f, -5f);
    public Vector2 maxBounds = new(5f, 5f);

    [Space]
    public bool useSideMagnet;
    public bool invert;
    
    private void Awake()
    {
        if (useSideMagnet) RecalcBounds();
    }

    private void RecalcBounds()
    {
        Camera cam = Camera.main;
        float zDistance = Mathf.Abs(cam.WorldToViewportPoint(transform.position).z);
        Vector3 worldMax = cam.ViewportToWorldPoint(new Vector3(1, 1, zDistance));
        Vector3 localMax = transform.parent.InverseTransformPoint(worldMax);
        var shift = localMax.x - 10.26399200013f;

        minBounds.x += shift / 2;
        maxBounds.x += shift / 2;

        if (invert)
        {
            var x1 = -minBounds.x;
            var x2 = -maxBounds.x;
            minBounds.x = x2;
            maxBounds.x = x1;
        }
    }

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