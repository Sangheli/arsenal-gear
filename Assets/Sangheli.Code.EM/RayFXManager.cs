using UnityEngine;

public class RayFXManager : MonoBehaviour
{
    [Header("2D-границы (XY)")]
    [Tooltip("Минимальные XY координаты")] public Vector2 minXY = new Vector2(-5, -5);
    [Tooltip("Максимальные XY координаты")] public Vector2 maxXY = new Vector2(5, 5);

    [Header("Фиксированная Z-координата для цели")] 
    public float targetZ = 0f;

    [Header("Префабы для рендера")]
    [Tooltip("LineRenderer для мгновенной линии")] public LineRenderer linePrefab;

    [Header("Общие настройки")] 
    [Tooltip("Автоуничтожение временных объектов, сек")] public float autoDestroyDelay = 2f;

    public float thickness = 0.1f;
    public GameObject laserGO;
    
    public Vector3 GetRandomTarget(Vector3 start)
    {
        var minusX = start.x < 0 ? -1 : 1;
        var minusY = start.y < 0 ? -1 : 1;
        
        float x = Random.Range(minXY.x, maxXY.x);
        float y = Random.Range(minXY.y, maxXY.y);
        return new Vector3(x * minusX, y*minusY, targetZ);
    }

    /// <summary>
    /// Стреляет лучом через TrailRenderer, двигая эмиттер из start в случайную цель.
    /// </summary>
    public void ShootTrailOld(Vector3 start)
    {
        if (linePrefab == null)
        {
            Debug.LogWarning("Line prefab is not assigned");
            return;
        }

        Vector3 end = GetRandomTarget(start);

        LineRenderer line = Instantiate(linePrefab);
        line.positionCount = 2;
        line.useWorldSpace = true;
        line.SetPosition(0, start);
        line.SetPosition(1, end);

        // Destroy the line after 0.2 seconds
        Destroy(line.gameObject, autoDestroyDelay);
    }

    //cylinder
    public void ShootTrail(Vector3 start)
    {
        var end = GetRandomTarget(start);

        GameObject beam = Instantiate(laserGO);
        Vector3 dir = end - start;
        float length = dir.magnitude;

        // позиция посередине
        beam.transform.position = start + dir / 2f;

        // направление
        beam.transform.rotation = Quaternion.FromToRotation(Vector3.up, dir);

        // масштаб по длине и толщине
        beam.transform.localScale = new Vector3(thickness, length / 2f, thickness);

        Destroy(beam, autoDestroyDelay);
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Vector3 a = new Vector3(minXY.x, minXY.y, targetZ);
        Vector3 b = new Vector3(maxXY.x, minXY.y, targetZ);
        Vector3 c = new Vector3(maxXY.x, maxXY.y, targetZ);
        Vector3 d = new Vector3(minXY.x, maxXY.y, targetZ);
        Gizmos.DrawLine(a, b);
        Gizmos.DrawLine(b, c);
        Gizmos.DrawLine(c, d);
        Gizmos.DrawLine(d, a);
    }
}
