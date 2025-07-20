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
        // Геометрическая прогрессия для bossHp и bossReward
        float bossHpBase = 30f;
        int bossRewardBase = 100;
        int bossAttackBase = 5;
        
        int scaleIndex = Mathf.Max(0, i - 3);
        
        float progressionHp = Mathf.Pow(1.4f, scaleIndex);
        float progressionReward = Mathf.Pow(1.3f, scaleIndex);
        float progressionAttack = Mathf.Pow(1.1f, scaleIndex);
        
        int bossHp = Mathf.RoundToInt(bossHpBase * progressionHp);
        int bossReward = Mathf.RoundToInt(bossRewardBase * progressionReward);
        int bossAttack = Mathf.RoundToInt(bossAttackBase * progressionAttack);
        
        return new LevelConfig
        {
            levelId = i,
            enemies = GenerateEnemies(i),
            loseHpDuringRound = true,
            showBossAfterTimer = true,
            checkNextRoundFromSave = false,
            showGoToUpgradesButton = true,
            preSpawnEnemyCount = 10,
            roundTime = 30f,
            bossHp = bossHp,
            bossReward = bossReward,
            bossAttack = bossAttack
        };
    }

    private static List<LevelEnemyInfo> GenerateEnemies(int levelIndex)
    {
        var allEnemies = G.I.Data.enemyContainer.List;
        if (allEnemies.Count == 0) return default;

        // Сортируем врагов по "силе"
        var sortedEnemies = allEnemies.OrderBy(e => e.Health + e.DamagePerHit).ToList();

        // На ранних уровнях доступно меньше врагов, на поздних — все
        int minEnemies = Math.Min(2, sortedEnemies.Count); // минимум 2 врага всегда
        int maxEnemies = sortedEnemies.Count;
        float t = levelIndex / (float)(G.I.maxLevel - 1); // 0..1
        int allowedCount = Mathf.RoundToInt(Mathf.Lerp(minEnemies, maxEnemies, t));
        allowedCount = Mathf.Clamp(allowedCount, minEnemies, maxEnemies);

        var allowedEnemies = sortedEnemies.Take(allowedCount).ToList();

        // Добавляем сильного врага на малых уровнях
        if (allowedCount < maxEnemies)
        {
            int strongStart = sortedEnemies.Count / 2;
            var strongCandidates = sortedEnemies.Skip(strongStart).Except(allowedEnemies).ToList();
            if (strongCandidates.Count > 0)
            {
                var strongEnemy = strongCandidates[UnityEngine.Random.Range(0, strongCandidates.Count)];
                allowedEnemies.Add(strongEnemy);
            }
        }

        float minHp = allowedEnemies.Min(e => e.Health);
        float maxHp = allowedEnemies.Max(e => e.Health);
        float hpRange = Mathf.Max(maxHp - minHp, 0.01f);

        float minDmg = allowedEnemies.Min(e => e.DamagePerHit);
        float maxDmg = allowedEnemies.Max(e => e.DamagePerHit);
        float dmgRange = Mathf.Max(maxDmg - minDmg, 0.01f);

        float baseChance = 5f;
        float maxChance = 70f;

        return allowedEnemies.Select(enemy =>
        {
            float normalizedHp = (enemy.Health - minHp) / hpRange;
            float normalizedDmg = (enemy.DamagePerHit - minDmg) / dmgRange;
            float combined = (normalizedHp + normalizedDmg) / 2f;
            float chance01 = Mathf.Lerp(1f - combined, combined, t);
            float chance = baseChance + (maxChance - baseChance) * chance01;
            chance = Mathf.Clamp(chance, baseChance, maxChance);

            int spawnRate;
            // Если это добавленный сильный враг на малом уровне — даём средний spawnRate
            int strongStart = sortedEnemies.Count / 2;
            if (allowedCount < maxEnemies && sortedEnemies.IndexOf(enemy) >= strongStart)
                spawnRate = 13; // средний спавнрейт
            else
                spawnRate = UnityEngine.Random.Range(5, 22);

            return new LevelEnemyInfo
            {
                enemyId = enemy.EnemyId,
                spawnChance = (int)chance,
                spawnRate = spawnRate
            };
        }).ToList();
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