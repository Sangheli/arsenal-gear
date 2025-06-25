using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnergySpawner : MonoBehaviour
{
    public EnergyView prefab;
    public readonly List<EnergyView> EnergyList = new();
    
    private Camera _mainCamera;
    private Vector2 _screenMin;
    private Vector2 _screenMax;
    private Transform _energyParent;

    public void OnLoadInit()
    {
        _mainCamera = Camera.main;
        var energyParentGO = new GameObject("Energies");
        _energyParent = energyParentGO.transform;
        if (_mainCamera != null)
        {
            var min = _mainCamera.ViewportToWorldPoint(new Vector2(0f, 0f));
            var max = _mainCamera.ViewportToWorldPoint(new Vector2(1f, 1f));
            float paddingX = (max.x - min.x) * 0.05f;
            float paddingY = (max.y - min.y) * 0.05f;
            _screenMin = new Vector2(min.x + paddingX, min.y + paddingY);
            _screenMax = new Vector2(max.x - paddingX, max.y - paddingY);
        }
    }

    public void Spawn(Vector3 pos, EnemyData enemyData)
    {
        var energyCount =  enemyData.GetEnergy();
        for (var i = 0; i < energyCount; i++) CreateEnergy(pos);
    }

    private void CreateEnergy(Vector3 pos)
    {
        var offset = Random.insideUnitCircle * 2.5f;

        offset.x = Mathf.Sign(offset.x) * Mathf.Clamp(offset.x, 0.65f, 0.8f);
        offset.y = Mathf.Sign(offset.y) * Mathf.Clamp(offset.y, 0.65f, 0.8f);
        
        var spawnPos = ClampToScreenBounds((Vector2)pos + offset);
        var energy = G.I.ObjectPool.Get(prefab, spawnPos);
        energy.transform.SetParent(_energyParent);
        EnergyList.Add(energy);
        energy.Init();
    }

    private Vector2 ClampToScreenBounds(Vector2 pos)
    {
        pos.x = Mathf.Clamp(pos.x, _screenMin.x, _screenMax.x);
        pos.y = Mathf.Clamp(pos.y, _screenMin.y, _screenMax.y);
        return pos;
    }

    public void StartRound() => ClearAll();
    public void StopRound() => ClearAll();

    private void ClearAll()
    {
        foreach (var x in EnergyList.Where(x => x != null))
            G.I.ObjectPool.Return(x);
        
        EnergyList.Clear();
    }
}