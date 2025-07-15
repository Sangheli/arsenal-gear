using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

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
    private InputSystem_Actions _inputActions;
    private LevelConfig _currentLevelConfig;
    
    private void Awake()
    {
        _mainCamera = Camera.main;
        var enemiesParentGo = new GameObject("Enemies");
        _enemiesParent = enemiesParentGo.transform;
        
        // Инициализация нового Input System
        _inputActions = new InputSystem_Actions();
        _inputActions.Player.Enable();
        _inputActions.Player.Crouch.performed += OnDebugMinusHp;
    }

    private void OnDestroy()
    {
        if (_inputActions != null)
            _inputActions.Player.Crouch.performed -= OnDebugMinusHp;
    }

    private void OnDebugMinusHp(InputAction.CallbackContext context)
    {
        if (IsAlive)
            G.I.HpManager.Reduce(100, doBlink: true, ignoreShield: true);
    }
    
    private void Update()
    {
        if (!IsAlive) return;
        
        var diff = Time.deltaTime * 1;
        G.I.HpManager.Reduce(diff, doBlink: false, ignoreShield: true);
        if (G.I.HpManager.RoundHp <= 0) G.I.StopRun(EndRoundFadeEnum.Fail);
        
        if(!IsAlive) return;
        
        if (_currentLevelConfig == null) return;
        foreach (var info in _currentLevelConfig.enemies)
        {
            var enemy = G.I.Data.enemyContainer.List.FirstOrDefault(e => e.EnemyId == info.enemyId);
            if (enemy != null)
                TrySpawnEnemyWithRate(enemy, info);
        }
    }

    private void TrySpawnEnemyWithRate(EnemyData enemyData, LevelEnemyInfo info)
    {
        if (!_enemySpawnTimers.ContainsKey(enemyData))
            _enemySpawnTimers[enemyData] = Time.time + 60f / Mathf.Max(info.spawnRate, 0.1f);

        if (Time.time < _enemySpawnTimers[enemyData])
            return;

        _enemySpawnTimers[enemyData] = Time.time + 60f / Mathf.Max(info.spawnRate, 0.1f);
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

    public void OnStartRound()
    {
        _save = G.I.SaveProvider.Get();
        ClearAll();
        _enemySpawnTimers.Clear();
        _sortingOrderData ??= new SortingOrderData { ramka = 4, Start = 4, Max = 30000 };
        // Получить конфиг уровня
        if (G.I.Data.levelDataConfigSO.Levels is { Count: > 0 })
        {
            var level = Mathf.Clamp(_save.level, 0, G.I.Data.levelDataConfigSO.Levels.Count - 1);
            _currentLevelConfig = G.I.Data.levelDataConfigSO.Levels[level];
        }
        else
        {
            Debug.LogError("LevelData.Levels is empty!");
            _currentLevelConfig = null;
        }
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
        
        if (_currentLevelConfig == null)
		return;
		
        var value = 10;
        var num = Random.Range(Mathf.Max(value/2,1), value + 1);
        
        for (var i = 0; i < num; i++)
        {
            if (_availableEnemies.Count == 0) break;
            var enemyData = GetRandomEnemyByChance();
            if (enemyData != null)
                SpawnEnemy(enemyData, isPreSpawned: true);
        }
    }
    
    private EnemyData GetRandomEnemyByChance()
    {
        if (_currentLevelConfig == null) return null;
        float total = 0f;
        foreach (var info in _currentLevelConfig.enemies) total += info.spawnChance;
        float rand = Random.value * total;
        float acc = 0f;
        foreach (var info in _currentLevelConfig.enemies)
        {
            acc += info.spawnChance;
            if (rand <= acc)
                return G.I.Data.enemyContainer.List.FirstOrDefault(e => e.EnemyId == info.enemyId);
        }
        return null;
    }

    private void UpdateAvailableEnemies()
    {
        _availableEnemies.Clear();
        if (_currentLevelConfig == null) return;
        foreach (var info in _currentLevelConfig.enemies)
        {
            var enemy = G.I.Data.enemyContainer.List.FirstOrDefault(e => e.EnemyId == info.enemyId);
            if (enemy != null)
            {
                enemy.Init(_save);
                _availableEnemies.Add(enemy);
            }
        }
    }
    
    private float GetSpawnRatePerMinute()
    {
        return 60f;
    }
}