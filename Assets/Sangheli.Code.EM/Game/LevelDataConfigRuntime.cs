using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using UnityEngine;

public class LevelDataConfigRuntime
{
    private readonly Dictionary<int, LevelConfig> _levels = new();

    private const float BossHpBase = 15;
    private const int BossRewardBase = 100;
    private const float BossAttackBase = 3.75f;

    public LevelDataConfigRuntime()
    {
        float prevHp = 0; 
        float prevReward = 0;
        float prevAttack = 0;
        
        for (var i = 0; i < G.I.maxLevel; i++)
        {
            if (_levels.ContainsKey(i)) continue;
            if (TryLoadLevel(i)) continue;
            
            var level = GenerateRandomLevel(i, prevHp, prevReward, prevAttack);
            _levels[i] = level;
            SaveLevelToJson(i, level);

            int scaleIndex = Mathf.Max(0, i - 3);
            float t = scaleIndex / (float)(G.I.maxLevel - 1);
            prevHp += BossHpBase * G.I.progressionHpCurve.Evaluate(t);
            prevReward += BossRewardBase * G.I.progressionRewardCurve.Evaluate(t);
            prevAttack += BossAttackBase * G.I.progressionAttackCurve.Evaluate(t);
        }
    }
    
    private bool TryLoadLevel(int i)
    {
        var loadedLevel = LoadLevelFromJson(i);
        if (loadedLevel == null) return false;
        _levels[i] = loadedLevel;
        return true;
    }

    private LevelConfig GenerateRandomLevel(int i, float prevHp, float prevReward, float prevAttack)
    {
        int scaleIndex = Mathf.Max(0, i - 3);
        float t = scaleIndex / (float)(G.I.maxLevel - 1);
        float progressionHp = prevHp + BossHpBase * G.I.progressionHpCurve.Evaluate(t);
        float progressionReward = prevReward + BossRewardBase * G.I.progressionRewardCurve.Evaluate(t);
        float progressionAttack = prevAttack + BossAttackBase * G.I.progressionAttackCurve.Evaluate(t);
        
        int bossHp = Mathf.RoundToInt(progressionHp);
        int bossReward = Mathf.RoundToInt(progressionReward);
        int bossAttack = Mathf.RoundToInt(progressionAttack);
        
        return new LevelConfig
        {
            levelId = i,
            enemies = GenerateEnemies(i),
            loseHpDuringRound = true,
            showBossAfterTimer = true,
            checkNextRoundFromSave = false,
            showGoToUpgradesButton = true,
            showTerminateButton = true,
            preSpawnEnemyCount = 10,
            roundTime = 23.5f,
            bossHp = bossHp,
            bossReward = bossReward,
            bossAttack = bossAttack,
            bossEnemySpawnRateMultiplier = 5 // Default value for boss enemy spawn rate multiplier
        };
    }

    private static List<LevelEnemyInfo> GenerateEnemies(int levelIndex)
    {
        var allEnemies = G.I.Data.enemyContainer.List.Where(x=>!x.excludeFromLevelGeneration).ToList();
        if (allEnemies.Count == 0) return default;

        // Сортируем врагов по "силе"
        var sortedEnemies = allEnemies.OrderBy(e => e.Health + e.DamagePerHit).ToList();

        int strongStart = sortedEnemies.Count / 2;

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

        return allowedEnemies.Select(enemy => new LevelEnemyInfo
            {
                enemyId = enemy.EnemyId,
                spawnChance = GetChance(enemy, minHp, hpRange, minDmg, dmgRange, t, baseChance, maxChance),
                spawnRate = GetSpawnRate(levelIndex, enemy, allowedCount, maxEnemies, sortedEnemies, strongStart)
            }).ToList();
    }

    private static int GetChance(EnemyData enemy, float minHp, float hpRange, float minDmg, float dmgRange, float t,
        float baseChance, float maxChance)
    {
        float normalizedHp = (enemy.Health - minHp) / hpRange;
        float normalizedDmg = (enemy.DamagePerHit - minDmg) / dmgRange;
        float combined = (normalizedHp + normalizedDmg) / 2f;
        float chance01 = Mathf.Lerp(1f - combined, combined, t);
        float chance = baseChance + (maxChance - baseChance) * chance01;
        return (int) Mathf.Clamp(chance, baseChance, maxChance);
    }

    private static int GetSpawnRate(int levelIndex, EnemyData enemy, int allowedCount, int maxEnemies, List<EnemyData> sortedEnemies,
        int strongStart)
    {
        if (enemy.useCustomSpawnRate) return enemy.GetSpawnRate();
        if (allowedCount < maxEnemies && sortedEnemies.IndexOf(enemy) >= strongStart) return 5; // средний спавнрейт

        // Вычисляем максимальный spawnRate в зависимости от уровня
        int minSpawnRate = 30;
        int maxSpawnRate = 60;
        float tSpawn = Mathf.Clamp01(levelIndex / (float)(G.I.maxLevel - 1));
        int dynamicMaxSpawnRate = Mathf.RoundToInt(Mathf.Lerp(minSpawnRate, maxSpawnRate, tSpawn));
        int dynamicMinSpawnRate = Mathf.RoundToInt(Mathf.Lerp(22, 30, tSpawn)); // Нижняя граница теперь зависит от уровня
        return UnityEngine.Random.Range(dynamicMinSpawnRate, dynamicMaxSpawnRate + 1); // +1, т.к. верхняя граница не включается
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