using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using UnityEngine;

public class LevelDataConfigRuntime
{
    private readonly Dictionary<int, LevelConfig> _levels = new();

    public LevelDataConfigRuntime()
    {
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

    private static LevelConfig GenerateRandomLevel(int i)
    {
        return new LevelConfig
        {
            levelId = i,
            enemies = GenerateEnemies(),
            loseHpDuringRound = true,
            showBossAfterTimer = true,
            checkNextRoundFromSave = false,
            showGoToUpgradesButton = true,
            preSpawnEnemyCount = 10,
            roundTime = 45f,
        };
    }

    private static List<LevelEnemyInfo> GenerateEnemies()
    {
        var allEnemies = G.I.Data.enemyContainer.List;
        return allEnemies.Count == 0 ? default : allEnemies.Select(GetEnemy).ToList();
    }

    private static LevelEnemyInfo GetEnemy(EnemyData enemy) =>
        new()
        {
            enemyId = enemy.EnemyId,
            spawnChance = UnityEngine.Random.Range(0, 101),
            spawnRate = UnityEngine.Random.Range(0, 22)
        };

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
            var json = JsonUtility.ToJson(level, true); // pretty print
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
        if (jsonAsset != null) return ParseLevelConfig(jsonAsset.text, levelIndex);

        // 2. Fallback to file system
        var path = GetLevelJsonPath(levelIndex);
        if (!File.Exists(path)) return null;
        var json = File.ReadAllText(path);
        return ParseLevelConfig(json, levelIndex);
    }
    
    private static LevelConfig ParseLevelConfig(string json, int levelIndex)
    {
        try
        {
            var config = JsonUtility.FromJson<LevelConfig>(json);
            if (config != null) config.levelId = levelIndex;
            return config;
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to parse level {levelIndex} config: {e}");
            return null;
        }
    }
}