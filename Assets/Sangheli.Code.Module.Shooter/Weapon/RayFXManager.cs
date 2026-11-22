using System.Collections.Generic;
using UnityEngine;

public class RayFXManager : MonoBehaviour
{
    [System.Serializable]
    public struct RayPrefab
    {
        public ERayType type;
        public GameObject prefab;
    }
    
    [Header("Ray Prefabs")]
    [Tooltip("List of prefabs for different ray types")]
    public RayPrefab[] rayPrefabs;

    //Player coords
    [Header("2D-границы (XY)")]
    [Tooltip("Минимальные XY координаты")] public Vector2 minXY = new Vector2(-5, -5);
    [Tooltip("Максимальные XY координаты")] public Vector2 maxXY = new Vector2(5, 5);

    [Header("Фиксированная Z-координата для цели")] 
    public float targetZ = 0f;


    [Header("Общие настройки")] 
    [Tooltip("Автоуничтожение временных объектов, сек")] public float autoDestroyDelay = 2f;

    [HideInInspector]
    public GameObject laserGO;
    public float thickness = 0.1f;
    
    // Cache for ray prefabs
    private Dictionary<ERayType, GameObject> _rayPrefabCache;

    private void Awake()
    {
        // Cache the ray prefabs in a dictionary for faster access
        _rayPrefabCache = new Dictionary<ERayType, GameObject>();
        foreach (var rayPrefab in rayPrefabs)
        {
            if (rayPrefab.type != ERayType.None && rayPrefab.prefab != null)
            {
                _rayPrefabCache[rayPrefab.type] = rayPrefab.prefab;
            }
        }
    }
    
    public Vector3 GetRandomTarget(Vector3 start)
    {
        var anchor = GetAimAnchor();
        var minusX = start.x < anchor.x ? -1 : 1;
        var minusY = start.y < anchor.y ? -1 : 1;
        
        float x = Random.Range(minXY.x, maxXY.x);
        float y = Random.Range(minXY.y, maxXY.y);
        return new Vector3(x * minusX, y*minusY, targetZ) + anchor;
    }

    public void ShootToPlayer(Vector3 start, ERayType rayType)
    {
        var end = GetRandomTarget(start);
        Shoot(start,end, rayType);
    }
    
    public void Shoot(Vector3 start, Vector3 end, ERayType rayType = ERayType.Laser)
    {
        // Get the appropriate prefab based on ray type
        GameObject prefab;
        if (!_rayPrefabCache.TryGetValue(rayType, out prefab))
        {
            // Fallback to default laserGO if ray type not found
            prefab = laserGO;
        }
        
        // Instantiate the prefab
        GameObject beam = Instantiate(prefab);
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
        var anchor = GetAimAnchor();
        Vector3 a = new Vector3(minXY.x, minXY.y, targetZ) + anchor;
        Vector3 b = new Vector3(maxXY.x, minXY.y, targetZ) + anchor;
        Vector3 c = new Vector3(maxXY.x, maxXY.y, targetZ) + anchor;
        Vector3 d = new Vector3(minXY.x, maxXY.y, targetZ) + anchor;
        Gizmos.DrawLine(a, b);
        Gizmos.DrawLine(b, c);
        Gizmos.DrawLine(c, d);
        Gizmos.DrawLine(d, a);
    }
    
    private static Vector3 GetAimAnchor()
    {
        var cam = Application.isPlaying ? G.I.cameraMain : Camera.main;
        var result = cam != null ? cam.transform.position : Vector3.zero;
        result.z = 0;
        return result;
    }
}
