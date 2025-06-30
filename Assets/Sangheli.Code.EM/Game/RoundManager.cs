using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RoundManager : MonoBehaviour
{
    private readonly List<EnemyController> _enemyList = new();
    private readonly Dictionary<EnemyData, float> _enemySpawnTimers = new();
    private Camera _mainCamera;
    private readonly List<EnemyData> _availableEnemies = new();
    public bool IsAlive { get; set; }
    public bool IsAliveForPlay { get; set; }
    private DataSave _save;
    private Transform _enemiesParent;
    private SortingOrderData _sortingOrderData;
    
    private void Awake()
    {
        _mainCamera = Camera.main;
        var enemiesParentGo = new GameObject("Enemies");
        _enemiesParent = enemiesParentGo.transform;
    }
    
    private void Update()
    {
        if (!IsAlive) return;
        
        G.I.HpManager?.UpdateHp();
        G.I.EnergyManager.LateUpdate();
        G.I.ShieldManager.LateUpdate();
        
        var diff = Time.deltaTime * 1;
        if (G.I.HpManager != null) G.I.HpManager.Reduce(diff, doBlink: false, ignoreShield: true);
        TryEndRound();
        
        if(!IsAlive) return;
        
        foreach (var t in _availableEnemies) TrySpawnEnemy(t);
    }

    private void TryEndRound()
    {
        if (G.I.HpManager.RoundHp > 0) return;
        G.I.prestigeManager.ToggleActive(false);
        StartCoroutine(EndRoundSequence());
    }

    private System.Collections.IEnumerator EndRoundSequence()
    {
        var energy = G.I.EnergyManager.RoundEnergyCount;
        var totalEnergy = G.I.SaveProvider.Get().totalEnergy;
        var prestigeDelta = G.I.prestigeManager.RoundPoints;
        G.I.EnergyManager.OnEndRound();
        G.I.prestigeManager.OnEndRound();
        
        // Отключаем управление и UI сразу
        G.I.playerCursor.ToggleControl(false);
        IsAlive = false;
        IsAliveForPlay = false;
        
        // Ждём анимацию сбора энергии
        yield return G.I.EnergyManager.CollectAllEnergyOnScreen();
        
        G.I.playerCursor.Toggle(false);
        G.I.uiManager.Toggle(false);
        
        // Показываем меню окончания раунда
        yield return G.I.endRoundView.Show(energy,totalEnergy,prestigeDelta);
    }

    private void TrySpawnEnemy(EnemyData enemyData)
    {
        if (!_enemySpawnTimers.ContainsKey(enemyData))
            _enemySpawnTimers[enemyData] = Time.time + 60f / GetSpawnRatePerMinute();

        if (Time.time < _enemySpawnTimers[enemyData])
            return;

        _enemySpawnTimers[enemyData] = Time.time + 60f / GetSpawnRatePerMinute();
        SpawnEnemy(enemyData, isPreSpawned: true);
    }

    private void SpawnEnemy(EnemyData enemyData, bool isPreSpawned = false)
    {
        var spawnPosition = ScreenSpawnHelper.GetRandomScreenPosition(_mainCamera);
        var moveDirection = ScreenSpawnHelper.GetMovementDirection(_mainCamera, spawnPosition);
        var enemy = G.I.ObjectPool.Get(enemyData.prefab, spawnPosition);
        enemy.transform.SetParent(_enemiesParent); // Устанавливаем родителя
        _enemyList.Add(enemy);
        enemy.Init(enemyData, moveDirection, isPreSpawned: isPreSpawned);
        _sortingOrderData.Iterrate();
        enemy.view.SetSortingOrder(_sortingOrderData);
    }

    public void StartRound()
    {
        _save = G.I.SaveProvider.Get();
        ClearAll();
        _enemySpawnTimers.Clear();
        _sortingOrderData ??= new SortingOrderData { ramka = 4, Start = 4, Max = 30000 };
        PreSpawnEnemies();
        IsAlive = true;
        StopAllCoroutines();
        StartCoroutine(EnableIsAliveForPlayDelayed());
    }

    private System.Collections.IEnumerator EnableIsAliveForPlayDelayed()
    {
        yield return new WaitForSeconds(0.5f);
        IsAliveForPlay = true;
    }

    public void ClearAll()
    {
        foreach (var x in _enemyList.Where(x => x != null)) 
            G.I.ObjectPool.Return(x);
        _enemyList.Clear();
    }

    private void PreSpawnEnemies()
    {
        UpdateAvailableEnemies();
        
        var value = 1;
        var num = Random.Range(Mathf.Max(value/2,1), value + 1);
        
        for (var i = 0; i < num; i++)
        {
            if (_availableEnemies.Count == 0) break;
            var enemyData = _availableEnemies[Random.Range(0, _availableEnemies.Count)];
            SpawnEnemy(enemyData, isPreSpawned: true);
        }
    }
    
    private void UpdateAvailableEnemies()
    {
        var list = G.I.enemyContainer.List;
        _availableEnemies.Clear();
        foreach (var t in list)
        {
            t.Init(_save);
            _availableEnemies.Add(t);
        }
    }
    
    private float GetSpawnRatePerMinute()
    {
        return 60f;
    }
}