using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using UnityEngine;

public class LevelDataConfigRuntime
{
    private readonly Dictionary<int, LevelConfig> _levels = new();

    public LevelDataConfigRuntime(LevelDataConfigSO configSO)
    {
        if (configSO != null)
        {
            foreach (var level in configSO.Levels) _levels[level.levelId] = level;
        }
        
        for (var i = 0; i < G.I.maxLevel; i++)
        {
            if (_levels.ContainsKey(i)) continue;
            if (!TryLoadLevel(i)) TrySaveLevel(i);
        }
    }
    
    private bool TryLoadLevel(int i)
    {
        var loadedLevel = LoadLevelFromJson(i);
        if (loadedLevel == null) return false;
        _levels[i] = loadedLevel;
        return true;
    }
    
    private void TrySaveLevel(int i)
    {
        var newLevel = GenerateRandomLevel(i);
        _levels[i] = newLevel;
        SaveLevelToJson(i, newLevel);
    }

    private static float GetEnemyDifficulty(EnemyData enemy)
    {
        // Можно скорректировать веса под баланс
        return enemy.Health * 0.5f + enemy.DamagePerHit * 1.0f + enemy.Speed * 0.3f + (enemy.AttractToCursor ? 10f : 0f);
    }

    private static LevelConfig GenerateRandomLevel(int i)
    {
        return new LevelConfig
        {
            enemies = GenerateEnemyes(i),
            loseHpDuringRound = true,
            showBossAfterTimer = true,
            checkNextRoundFromSave = false,
            showGoToUpgradesButton = true,
            preSpawnEnemyCount = 10,
        };
    }
    // старый GenerateRandomLevel для обратной совместимости
    private static LevelConfig GenerateRandomLevel()
    {
        return GenerateRandomLevel(0);
    }

    private static List<LevelEnemyInfo> GenerateEnemyes(int levelIndex)
    {
        var allEnemies = G.I.Data.enemyContainer.List;
        if (allEnemies.Count == 0) return new List<LevelEnemyInfo>();

        var enemiesWithDifficulty = allEnemies
            .Select(e => new { Enemy = e, Difficulty = GetEnemyDifficulty(e) })
            .OrderBy(x => x.Difficulty)
            .ToList();

        float t = Mathf.Clamp01(levelIndex / (float)Mathf.Max(1, G.I.maxLevel - 1));
        int n = enemiesWithDifficulty.Count;
        float[] weights = new float[n];
        float sum = 0f;
        for (int i = 0; i < n; i++)
        {
            float normalized = i / (float)(n - 1);
            float baseWeight = Mathf.Lerp(1f - normalized, normalized, t);
            var enemy = enemiesWithDifficulty[i].Enemy;
            float energy = enemy.GetEnergy();
            bool nonAttacker = Mathf.Approximately(enemy.DamagePerHit, 0f);
            float maxEnergy = allEnemies.Max(e => e.GetEnergy());
            float energyScore = maxEnergy > 0 ? energy / maxEnergy : 0f;
            float bonus = 1f;
            if (nonAttacker && energyScore > 0.5f)
            {
                bonus += t * t * energyScore * 3f;
            }
            weights[i] = baseWeight * bonus;
            weights[i] = Mathf.Max(weights[i], 0.01f);
            sum += weights[i];
        }
        for (int i = 0; i < n; i++) weights[i] /= sum;

        // --- Количество типов врагов: от 50% до 90% от общего числа, округляем вверх ---
        int minTypes = Mathf.CeilToInt(n * 0.5f);
        int maxTypes = Mathf.CeilToInt(n * 0.9f);
        int numEnemies = UnityEngine.Random.Range(minTypes, maxTypes + 1);
        numEnemies = Mathf.Clamp(numEnemies, 1, n);

        var chosenIndices = new HashSet<int>();
        while (chosenIndices.Count < numEnemies)
        {
            float r = UnityEngine.Random.value;
            float acc = 0f;
            for (int i = 0; i < n; i++)
            {
                acc += weights[i];
                if (r <= acc)
                {
                    chosenIndices.Add(i);
                    break;
                }
            }
        }
        return chosenIndices
            .Select(x => enemiesWithDifficulty[x].Enemy)
            .Select(e => GenerateEnemy(e, levelIndex))
            .ToList();
    }
    // старый GenerateEnemyes для обратной совместимости
    private static List<LevelEnemyInfo> GenerateEnemyes()
    {
        return GenerateEnemyes(0);
    }

    private static LevelEnemyInfo GenerateEnemy(EnemyData enemy, int levelIndex = 0)
    {
        // spawnRate: на 0 уровне 60, на максимальном — случайно от 120 до 150, рост нелинейный
        float tRaw = levelIndex / (float)Mathf.Max(1, G.I.maxLevel - 1);
        float t = tRaw * tRaw; // нелинейный рост (ускоряющийся)
        float minSpawn = Mathf.Lerp(20, 120f, t);
        float maxSpawn = Mathf.Lerp(20, 150f, t);
        float spawnRate = UnityEngine.Random.Range(minSpawn, maxSpawn);
        return new LevelEnemyInfo
        {
            enemyId = enemy.EnemyId,
            spawnChance = UnityEngine.Random.Range(30f, 101f),
            spawnRate = spawnRate
        };
    }

    public LevelConfig Get(int levelId)
    {
        _levels.TryGetValue(levelId, out var config);
        return config;
    }

    public int Count() => _levels.Count;

    private static string GetLevelJsonPath(int levelIndex)
    {
#if UNITY_EDITOR
        var dir = Path.Combine(Application.dataPath, "Levels");
#else
        var dir = Path.Combine(Application.persistentDataPath, "Levels");
#endif
        if (!Directory.Exists(dir))
            Directory.CreateDirectory(dir);
        return Path.Combine(dir, $"Level_{levelIndex}.json");
    }

    private static void SaveLevelToJson(int levelIndex, LevelConfig level)
    {
        try
        {
            level.levelId = levelIndex;
            var json = JsonUtility.ToJson(new LevelConfigSerializable(level), true); // pretty print
            File.WriteAllText(GetLevelJsonPath(levelIndex), json);
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to save level {levelIndex} to JSON: {e}");
        }
    }

    private static LevelConfig LoadLevelFromJson(int levelIndex)
    {
        // 1. Try load from Resources
        string resourcePath = $"Levels/Level_{levelIndex}"; // Resources/Levels/Level_X.json (without .json)
        TextAsset jsonAsset = Resources.Load<TextAsset>(resourcePath);
        if (jsonAsset != null)
        {
            try
            {
                var serializable = JsonUtility.FromJson<LevelConfigSerializable>(jsonAsset.text);
                var config = serializable?.ToLevelConfig();
                if (config != null) config.levelId = levelIndex;
                return config;
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to load level {levelIndex} from Resources: {e}");
            }
        }

        // 2. Fallback to file system
        var path = GetLevelJsonPath(levelIndex);
        if (!File.Exists(path))
            return null;
        try
        {
            var json = File.ReadAllText(path);
            var serializable = JsonUtility.FromJson<LevelConfigSerializable>(json);
            var config = serializable?.ToLevelConfig();
            if (config != null) config.levelId = levelIndex;
            return config;
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to load level {levelIndex} from JSON: {e}");
            return null;
        }
    }

    [Serializable]
    private class LevelConfigSerializable
    {
        public int levelId;
        public bool loseHpDuringRound;
        public bool showBossAfterTimer;
        public bool checkNextRoundFromSave;
        public bool showGoToUpgradesButton;
        public int preSpawnEnemyCount;
        public List<LevelEnemyInfo> enemies;

        public LevelConfigSerializable(LevelConfig config)
        {
            levelId = config.levelId;
            loseHpDuringRound = config.loseHpDuringRound;
            showBossAfterTimer = config.showBossAfterTimer;
            checkNextRoundFromSave = config.checkNextRoundFromSave;
            showGoToUpgradesButton = config.showGoToUpgradesButton;
            preSpawnEnemyCount = config.preSpawnEnemyCount;
            enemies = config.enemies;
        }

        public LevelConfig ToLevelConfig()
        {
            var config = ScriptableObject.CreateInstance<LevelConfig>();
            config.levelId = levelId;
            config.loseHpDuringRound = loseHpDuringRound;
            config.showBossAfterTimer = showBossAfterTimer;
            config.checkNextRoundFromSave = checkNextRoundFromSave;
            config.showGoToUpgradesButton = showGoToUpgradesButton;
            config.preSpawnEnemyCount = preSpawnEnemyCount;
            config.enemies = enemies;
            return config;
        }
    }
}