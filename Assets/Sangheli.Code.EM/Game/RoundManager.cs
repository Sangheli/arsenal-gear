using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class RoundManager : MonoBehaviour
{
    private readonly List<EnemyController> _enemyList = new();
    private readonly Dictionary<EnemyData, float> _enemySpawnTimers = new();
    private readonly List<EnemyData> _availableEnemies = new();

    // List to track enemies that are currently allowed to attack
    private readonly List<EnemyController> _attackingEnemies = new();

    private readonly List<IRoundModification> _roundModifications = new()
    {
        new PoisonRoundModification(),
    };
        
    private readonly List<IRoundModification> _activeRoundModifications = new();
    
    public bool IsAlive { get; set; }
    public bool IsAliveForPlay { get; set; }
    private DataSave _save;
    private Transform _enemiesParent;
    private SortingOrderData _sortingOrderData;
    private InputSystem_Actions _inputActions;
    public LevelConfig _currentLevelConfig { get; private set; }
    
    private bool IsBossActive { get; set; }
    public void SetBossActive(bool active) => IsBossActive = active;

    public SpawnPositionManager _spawnPositionManager = new();
    private bool _attackDisabled;

    private void Awake()
    {
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
        if (!IsAlive)
        {
            DisableAllEnemyAttacking();
            return;
        }

        HandleCursorVisibility();
        
        foreach (var modification in _activeRoundModifications)
            modification.OnRoundUpdate(this);

        if (G.I.HpManager.RoundHp <= 0)
        {
            G.I.Container.pooledAudioPlayer.PlayWithRandomPitch(G.I.Data.soundContainer.gameOver);
            G.I.StopRun(null, EndRoundFadeEnum.RoundFail);
        }
        
        if(!IsAlive)
        {
            DisableAllEnemyAttacking();
            return;
        }
        
        if (_currentLevelConfig == null) return;
        foreach (var info in _currentLevelConfig.enemies)
        {
            var enemy = G.I.Data.enemyContainer.List.FirstOrDefault(e => e.EnemyId == info.enemyId);
            // Skip enemies with spawnAtStart flag - they should only be spawned at the start
            if (enemy != null && !enemy.spawnAtStart)
                TrySpawnEnemyWithRate(enemy, info);
        }
        
        // Update attacking enemies list
        UpdateAttackingEnemies();
    }
    
    private static void HandleCursorVisibility()
    {
        if (EventSystem.current != null)
            CursorController.ToggleSystemCursor(EventSystem.current.IsPointerOverGameObject());
    }

    private void TrySpawnEnemyWithRate(EnemyData enemyData, LevelEnemyInfo info)
    {
        float effectiveSpawnRate = Mathf.Max(info.spawnRate, 0.1f) * GetSpawnRateMultiplier();
        
        // Reduce spawn rate by the multiplier from level config when boss is active
        if (IsBossActive && _currentLevelConfig != null)
            effectiveSpawnRate /= _currentLevelConfig.bossEnemySpawnRateMultiplier;

        if (!_enemySpawnTimers.ContainsKey(enemyData))
            _enemySpawnTimers[enemyData] = Time.time + 60f / effectiveSpawnRate;

        if (Time.time < _enemySpawnTimers[enemyData])
            return;

        _enemySpawnTimers[enemyData] = Time.time + 60f / effectiveSpawnRate;
        SpawnEnemy(enemyData,info.difficult, isPreSpawned: true);
    }

    private static float GetSpawnRateMultiplier() => 1.0f + 0.35f * G.I.levelTargetManager.Data.End.GetLevelPercent();

    private void SpawnEnemy(EnemyData enemyData,int difficult, bool isPreSpawned = false)
    {
        if(!_spawnPositionManager.GetPosition(enemyData.PosConfig, out var spawnPosition, out var zPosition)) return;
        
        var moveDirection = ScreenSpawnHelper.GetMovementDirection(spawnPosition);
        var enemy = G.I.ObjectPool.Get(enemyData.prefab, spawnPosition);
        enemy.transform.SetParent(_enemiesParent); // Устанавливаем родителя
        _enemyList.Add(enemy);
        enemy.Init(enemyData, moveDirection,difficult: difficult,_currentLevelConfig.levelId, isPreSpawned: isPreSpawned);
        _sortingOrderData.Iterrate();
        enemy.view.SetSortingOrder(_sortingOrderData);
        
        if (!enemyData.PosConfig.ignoreSpawnPositionManager)
        {
            var zPosHandler = (IZPosHandler)enemy;
            zPosHandler.ZPos = zPosition;
        }
        
        // Initially disable attack for all enemies
        enemy.CanAttack = false;
    }

    public void OnStartRound()
    {
        _save = G.I.SaveProvider.Get();
        _attackDisabled = false;
        ClearAll();
        _enemySpawnTimers.Clear();
        _sortingOrderData ??= new SortingOrderData { ramka = 4, Start = 4, Max = 15000 };
        _currentLevelConfig = G.I.LevelDataConfig.Get(_save.level);
        
        _activeRoundModifications.Clear();
        if (_currentLevelConfig != null)
        {
            foreach (var modifierType in _currentLevelConfig.levelModifiers)
            {
                var modifier = _roundModifications.FirstOrDefault(m => m.ModifierType == modifierType);
                if (modifier != null)
                {
                    _activeRoundModifications.Add(modifier);
                    modifier.OnRoundStart(this);
                }
            }
        }
        
        // Boss is not active at the start of round
        IsBossActive = false;
        
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
        _attackingEnemies.Clear();
        _spawnPositionManager.Clear();
        
        // Reset boss active state when clearing all enemies
        IsBossActive = false;
    }

    private void PreSpawnEnemies()
    {
        UpdateAvailableEnemies();
        
        if (_currentLevelConfig == null)
		    return;
        var filteredEnemiesForPreSpawn = GetFilteredEnemiesForPreSpawn();

        // Spawn enemies with spawnAtStart flag
        foreach (var enemyData in _availableEnemies.Where(e => e.spawnAtStart))
        {
            SpawnEnemy(enemyData, difficult: 0, isPreSpawned: true);
        }

        for (var i = 0; i < _currentLevelConfig.preSpawnEnemyCount; i++)
        {
            if (filteredEnemiesForPreSpawn.Count == 0) break;
            var enemyData = GetRandomEnemyByChanceForPreSpawn(filteredEnemiesForPreSpawn);
            if (enemyData != null)
                SpawnEnemy(enemyData, difficult:0, isPreSpawned: true);
        }
    }
    
    private List<LevelEnemyInfo> GetFilteredEnemiesForPreSpawn() =>
        _currentLevelConfig.enemies
            .Select(info => (info, enemy: G.I.Data.enemyContainer.List.FirstOrDefault(e => e.EnemyId == info.enemyId)))
            .Select(x => x.info).ToList();

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
    
    private EnemyData GetRandomEnemyByChanceForPreSpawn(List<LevelEnemyInfo> filteredEnemies)
    {
        if (filteredEnemies.Count == 0) return null;
        
        float total = 0f;
        foreach (var info in filteredEnemies) total += info.spawnChance;
        float rand = Random.value * total;
        float acc = 0f;
        foreach (var info in filteredEnemies)
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

    public void TerminateRound()
    {
        if (!IsAlive) return;
        
        // Disable all enemies from attacking when round terminates
        DisableAllEnemyAttacking();
        
        // Notify modifiers about round end
        foreach (var modification in _activeRoundModifications)
            modification.OnRoundEnd(this);
        
        G.I.Container.pooledAudioPlayer.PlayWithRandomPitch(G.I.Data.soundContainer.gameOver);
        G.I.StopRun(null, EndRoundFadeEnum.RoundTerminated);
    }
    
    // Called when an enemy is destroyed to potentially allow another enemy to attack
    public void OnEnemyDestroyed(EnemyController enemy)
    {
        if (_attackingEnemies.Contains(enemy))
        {
            _attackingEnemies.Remove(enemy);
            AssignNewAttackingEnemy();
        }
    }
    
    // Disable all enemies from attacking
    public void DisableAllEnemyAttacking()
    {
        if(_attackDisabled) return;
        _attackDisabled = true;
        
        foreach (var enemy in _attackingEnemies.Where(enemy => enemy != null && enemy.gameObject.activeInHierarchy)) enemy.CanAttack = false;
        _attackingEnemies.Clear();
    }
    
    // Update the list of attacking enemies
    private void UpdateAttackingEnemies()
    {
        // Remove any null or destroyed enemies
        _attackingEnemies.RemoveAll(e => e == null || !e.gameObject.activeInHierarchy);
        
        // If we have less than the max attacking enemies, try to assign more
        while (_attackingEnemies.Count < _currentLevelConfig.MaxAttackingEnemies && AssignNewAttackingEnemy())
        {
            // Keep assigning until we reach the limit or run out of candidates
        }
    }
    
    // Assign a new enemy to be an attacker
    private bool AssignNewAttackingEnemy()
    {
        // Find all active enemies that are not currently attacking
        var nonAttackingEnemies = _enemyList
            .Where(e => e != null && e.gameObject.activeInHierarchy && !_attackingEnemies.Contains(e) && e.CanAttack != true)
            .ToList();
        
        if (nonAttackingEnemies.Count == 0)
            return false;
        
        // Find the closest enemy to the player
        EnemyController closestEnemy = null;
        float closestDistance = float.MaxValue;
        Vector3 playerPosition = G.I.playerCursor.transform.position;
        
        foreach (var enemy in nonAttackingEnemies)
        {
            if(enemy.DamageValue<0.0001f)
                continue;
            
            float distance = Vector3.Distance(enemy.transform.position, playerPosition);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestEnemy = enemy;
            }
        }
        
        if (closestEnemy != null)
        {
            closestEnemy.CanAttack = true;
            _attackingEnemies.Add(closestEnemy);
            return true;
        }
        
        return false;
    }
}