using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnergySpawner : MonoBehaviour
{
    public EnergyView prefab;
    public readonly HashSet<EnergyView> EnergyList = new();
    
    [Header("Настройки монет")]
    public float spawnInterval = 0.01f;  // Интервал между выстрелами
    public float force = 5f;             // Сила "фонтана"
    public float upwardForce = 5f;       // Сила вверх
    public float fixedZ = 0f;            // Фиксированная позиция по Z
    public float floorY = -4f;
    
    private Camera _mainCamera;
    private Vector2 _screenMin;
    private Vector2 _screenMax;
    private Transform _energyParent;
    private EnergySortingOrderData _sortingOrderData = new() { view = 15002, shadow = 15001, Max = 30000, Start = 15002 };

    public void OnLoadInit()
    {
        _mainCamera = Camera.main;
        _energyParent = new GameObject("Energies").transform;
    }

    private void CalcBound()
    {
        var min = _mainCamera.ViewportToWorldPoint(new Vector2(0f, 0f));
        var max = _mainCamera.ViewportToWorldPoint(new Vector2(1f, 1f));
        var paddingX = (max.x - min.x) * 0.05f;
        var paddingY = (max.y - min.y) * 0.05f;
        _screenMin = new Vector2(min.x + paddingX, min.y + paddingY);
        _screenMax = new Vector2(max.x - paddingX, max.y - paddingY);
    }

    public void Spawn(Vector3 pos, EnemyData enemyData)
    {
        pos = G.I.cameraMain.WorldToScreenPoint(pos+enemyData.SpawnCoinShift);
        pos = ScreenToWorldOnPlane(pos);
        if(enemyData.GetEnergy() < 1) return;
        StartCoroutine(SpawnCoins(pos, enemyData));
    }

    private Vector3 ScreenToWorldOnPlane(Vector2 screenPos, float zPlane = 0f)
    {
        var ray = G.I.cameraMain.ScreenPointToRay(screenPos);
        var plane = new Plane(Vector3.forward, new Vector3(0, 0, zPlane));
        return plane.Raycast(ray, out var distance) ? ray.GetPoint(distance) : Vector3.zero;
    }

    public Vector3 WorldToScreen(Vector3 worldPos)
    {
        // Получаем позицию в экранных координатах
        var screenPos = G.I.cameraMain.WorldToScreenPoint(worldPos);
        // screenPos.z — это расстояние до камеры (если < 0, то объект "за камерой")
        return screenPos;
    }

    private EnergyView CreateEnergy(Vector3 pos)
    {
        Vector3 screenPosition = G.I.cameraMain.WorldToScreenPoint(pos);
        // Vector3 screenPos = Input.mousePosition; // или любая точка на экране
        var transform1 = G.I.cameraMain.transform;
        float distance = -transform1.position.z / transform1.forward.z;
        Vector3 worldPos = G.I.cameraMain.ScreenToWorldPoint(new Vector3(screenPosition.x, screenPosition.y, distance));

        // var energy = G.I.ObjectPool.Get(prefab, GetPosRandom2D(pos));
        var energy = G.I.ObjectPool.Get(prefab, worldPos);
        energy.transform.SetParent(_energyParent);
        energy.Init();
        energy.SetSortingOrder(_sortingOrderData);
        _sortingOrderData.Iterate();
        EnergyList.Add(energy);

        return energy;
    }
    
    private System.Collections.IEnumerator SpawnCoins(Vector3 pos, EnemyData enemyData)
    {
        var energyCount =  enemyData.GetEnergy();
        int intervals = (int)Mathf.Min(energyCount, 10);
        int coinsPerInterval = (int)(energyCount / intervals);
        int extraCoins = (int) energyCount % intervals;
        
        for (int i = 0; i < intervals; i++)
        {
            int coinsToSpawn = coinsPerInterval + (i < extraCoins ? 1 : 0);
            for (int j = 0; j < coinsToSpawn; j++)
            {
                SpawnCoin(pos);
            }
            yield return new WaitForSeconds(spawnInterval);
        }
    }
    
    private void SpawnCoin(Vector3 pos)
    {
        pos.z = fixedZ;
        var coin = CreateEnergy(pos);
        coin.coinFloorHandler.SetImpulse(force, upwardForce, floorY);
    }

    private Vector2 GetPosRandom2D(Vector3 pos)
    {
        var offset = Random.insideUnitCircle * 2.5f;

        offset.x = Mathf.Sign(offset.x) * Mathf.Clamp(offset.x, 0.65f, 0.8f);
        offset.y = Mathf.Sign(offset.y) * Mathf.Clamp(offset.y, 0.65f, 0.8f);

        return ClampToScreenBounds((Vector2)pos + offset);
    }

    private Vector2 ClampToScreenBounds(Vector2 pos)
    {
        pos.x = Mathf.Clamp(pos.x, _screenMin.x, _screenMax.x);
        pos.y = Mathf.Clamp(pos.y, _screenMin.y, _screenMax.y);
        return pos;
    }

    public void OnStartRound()
    {
        if (_mainCamera != null) CalcBound();
        ClearAll();
    }

    public void StopRound() => ClearAll();

    private void ClearAll()
    {
        foreach (var x in EnergyList.Where(x => x != null))
            G.I.ObjectPool.Return(x);
        
        EnergyList.Clear();
    }
}