using UnityEngine;
using UnityEngine.InputSystem;

public class ClampPosition : MonoBehaviour
{
    [Header("Границы движения")]
    public Vector2 minBounds = new(-5f, -5f);
    public Vector2 maxBounds = new(5f, 5f);

    [Space]
    public bool useSideMagnet;
    public bool invert;

    [Header("Плавность движения")]
    [Range(0.01f, 20f)] public float moveSpeed = 10f;

    private const float ConstX = 10.26399200013f;

    private Vector3 targetWorldPos; // сюда будем сохранять целевую позицию

    private void Awake()
    {
        if (useSideMagnet) RecalcBounds();

        // чтобы стартовать без рывка
        targetWorldPos = transform.position;
    }

    private void Update()
    {
        // Плавно двигаем объект к targetWorldPos
        transform.position = Vector3.Lerp(
            transform.position,
            targetWorldPos,
            Time.deltaTime * moveSpeed
        );
    }

    private void RecalcBounds()
    {
        Camera cam = Camera.main;
        float zDistance = Mathf.Abs(cam.WorldToViewportPoint(transform.position).z);
        Vector3 worldMax = cam.ViewportToWorldPoint(new Vector3(1, 1, zDistance));
        Vector3 localMax = transform.parent.InverseTransformPoint(worldMax);
        var shift = localMax.x - ConstX;

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
        var worldPos = targetPos;
        
        // Ограничиваем X и Y
        float clampedX = Mathf.Clamp(worldPos.x, minBounds.x, maxBounds.x);
        float clampedY = Mathf.Clamp(worldPos.y, minBounds.y, maxBounds.y);

        // Z оставляем от targetPos
        targetWorldPos = new Vector3(clampedX, clampedY, transform.position.z);
    }
    
    /// <summary>
    /// Устанавливает целевую позицию (объект двигается плавно).
    /// </summary>
    public void SetPosition(Vector2 screenPos)
    {
        var objPos = transform.position;
        var cam = G.I.cameraMain.transform;
        var distance = Vector3.Dot(objPos - cam.position, cam.forward);
        var screenPoint = new Vector3(screenPos.x, screenPos.y, distance);
        var worldPos = G.I.cameraMain.ScreenToWorldPoint(screenPoint);

        // Ограничиваем X и Y
        float clampedX = Mathf.Clamp(worldPos.x, minBounds.x, maxBounds.x);
        float clampedY = Mathf.Clamp(worldPos.y, minBounds.y, maxBounds.y);

        // целевая позиция (Z оставляем)
        targetWorldPos = new Vector3(clampedX, clampedY, transform.position.z);
    }
}