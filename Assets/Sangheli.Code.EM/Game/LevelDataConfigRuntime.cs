using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using UnityEngine;
using Random = UnityEngine.Random;

public class LevelDataConfigRuntime
{
    private readonly Dictionary<int, LevelConfig> _levels = new();

    private const int BossRewardBase = 100;
    private const int SkipTutorialLevels = 5;

    public LevelDataConfigRuntime()
    {
        float prevReward = 0;
        
        for (var i = 0; i < G.I.maxLevel; i++)
        {
            if (_levels.ContainsKey(i)) continue;
            if (TryLoadLevel(i)) continue;
            
            var level = GenerateRandomLevel(i, prevReward);
            _levels[i] = level;
            SaveLevelToJson(i, level);

            prevReward += BossRewardBase * G.I.progressionRewardCurve.Evaluate(GetScaleValue(i));
        }
    }
    
    private bool TryLoadLevel(int i)
    {
        var loadedLevel = LoadLevelFromJson(i);
        if (loadedLevel == null) return false;
        _levels[i] = loadedLevel;
        return true;
    }

    private LevelConfig GenerateRandomLevel(int i, float prevReward)
    {
        var scale = GetScaleValue(i);
        var objectiveType = GetObjectiveType(i);
        var playerDps = GetPlayerDps(scale);
        var bossHp = CalculateBossHp(scale, playerDps);
        
        return new LevelConfig
        {
            levelId = i,
            enemies = GenerateEnemies(scale),
            showHpBar = true,
            checkNextRoundFromSave = false,
            showGoToUpgradesButton = true,
            showTerminateButton = true,
            preSpawnEnemyCount = 10,
            roundTime = 23.5f,
            bossHp = bossHp,
            levelReward =  GetLevelReward(objectiveType, prevReward, scale),
            bossAttack = CalculateBossAttack(scale, playerDps , bossHp),
            bossEnemySpawnRateMultiplier = 5, // Default value for boss enemy spawn rate multiplier
            useWinColorOnFail = false, // By default, use normal fail color on defeat
            playerAutoAttack = true, // By default, player auto-attack is enabled
            objectiveType = objectiveType,
            objectiveValue = GetObjectiveValue(objectiveType, scale),
            levelModifiers = GenerateRandomModifiers(scale) // Generate random modifiers for the level
        };
    }

    private static int GetChance(EnemyData enemy, float minHp, float hpRange, float minDmg, float dmgRange, float scale,
        float baseChance, float maxChance)
    {
        float normalizedHp = (enemy.Health - minHp) / hpRange;
        float normalizedDmg = (enemy.DamagePerHit - minDmg) / dmgRange;
        float combined = (normalizedHp + normalizedDmg) / 2f;
        float chance01 = Mathf.Lerp(1f - combined, combined, scale);
        float chance = baseChance + (maxChance - baseChance) * chance01;
        return (int) Mathf.Clamp(chance, baseChance, maxChance);
    }

    private static int GetSpawnRate(float scale, EnemyData enemy)
    {
        if (enemy.useCustomSpawnRate) return enemy.GetSpawnRate();
        var rate = GetDynamicSpawnRate(scale);
        return enemy.superHard ? (int)Mathf.Lerp(5, rate, scale) : rate;
    }

    private static List<LevelModifierType> GenerateRandomModifiers(float scale)
    {
        return new List<LevelModifierType>()
        {
            // LevelModifierType.Poison,
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

    private static int GetObjectiveValue(LevelObjectiveType objectiveType, float t)
    {
        switch (objectiveType)
        {
            case LevelObjectiveType.CoinCount:
                int coinMin = Random.Range(25, 30);
                int coinMax = Random.Range(2500, 3000);
                return Mathf.RoundToInt(Mathf.Lerp(coinMin, coinMax, t));
            case LevelObjectiveType.EnemyCount:
                int min = Random.Range(15, 25);
                int max = Random.Range(250, 350);
                return Mathf.RoundToInt(Mathf.Lerp(min, max, t));
            case LevelObjectiveType.Boss:
                return Random.Range(50, 3000); //virtual distance to boss
            case LevelObjectiveType.Time:
                return 0; // For time objective, we use the roundTime property
            default:
                return 0;
        }
    }

    private static LevelObjectiveType GetObjectiveType(int i)
    {
        // Generate random objective
        if ((i + 1) % 5 == 0) 
            return LevelObjectiveType.Boss;
        
        // Generate list of non-boss objectives
        var nonBossObjectives = Enum.GetValues(typeof(LevelObjectiveType))
            .Cast<LevelObjectiveType>()
            .Where(type => type != LevelObjectiveType.Boss && type != LevelObjectiveType.None)
            .ToArray();
            
        // Select random objective from non-boss types
        return nonBossObjectives[Random.Range(0, nonBossObjectives.Length)];
    }

    private static int CalculateBossHp(float t, float playerDps)
    {
        // Calculate boss health based on desired fight duration (14-17 seconds)
        float minDuration = 14f;
        float maxDuration = 17f;
        float fightDuration = Mathf.Lerp(minDuration, maxDuration, t);

        // Calculate boss health based on player DPS and desired fight duration
        int bossHp = Mathf.RoundToInt(playerDps * fightDuration * 1.2f);
        return bossHp;
    }

    private static float GetPlayerDps(float scale)
    {
        // Player DPS progresses from 5 to 225
        float minDps = 5f;
        float maxDps = 225f;
        return Mathf.Lerp(minDps, maxDps, scale);
    }

    // New method to calculate boss attack based on player's shield progression
    private static int CalculateBossAttack(float scale, float playerDps, float bossHp)
    {
        // Player's HP progression: 15 (min) to 255 (max)
        float minPlayerHp = 15f;
        float maxPlayerHp = 255f;
        float currentPlayerHp = Mathf.Lerp(minPlayerHp, maxPlayerHp, scale);

        // Player's shield progression: 0 (min) to 250 (max)
        float minShieldCapacity = 0f;
        float maxShieldCapacity = 250f;
        float currentShieldCapacity = Mathf.Lerp(minShieldCapacity, maxShieldCapacity, scale);

        // Shield regeneration: 0 (min) to 1.25 per second (max)
        float minShieldRegen = 0f;
        float maxShieldRegen = 1.25f;
        float currentShieldRegen = Mathf.Lerp(minShieldRegen, maxShieldRegen, scale);

        // Effective player defense = HP + shield + regen contribution
        float baseDefense = currentPlayerHp + currentShieldCapacity;

        // Player time to kill boss
        float playerTimeToKillBoss = bossHp / playerDps;

        // Boss should kill player 0.5%-1% faster
        float minTimeReduction = 0.05f;
        float maxTimeReduction = 0.1f;
        float timeReduction = Mathf.Lerp(minTimeReduction, maxTimeReduction, scale);

        float targetKillTime = playerTimeToKillBoss * (1f - timeReduction);

        // Effective defense grows with regen over kill time
        float effectiveDefense = baseDefense + currentShieldRegen * targetKillTime;

        // Boss attack per second required
        float requiredBossAttack = effectiveDefense / targetKillTime;

        return Mathf.CeilToInt(requiredBossAttack);
    }

    private static float GetScaleValue(int i)
    {
        var scaleIndex = Mathf.Max(0, i - SkipTutorialLevels);
        return scaleIndex / (float)(G.I.maxLevel - SkipTutorialLevels - 1);
    }
    
    private static int GetLevelReward(LevelObjectiveType objectiveType, float prevReward, float t)
    {
        if (objectiveType != LevelObjectiveType.Boss)
            return 0;
        
        float progressionReward = prevReward + BossRewardBase * G.I.progressionRewardCurve.Evaluate(t);
        return Mathf.RoundToInt(progressionReward);
    }
    
    private List<LevelEnemyInfo> GenerateEnemies(float scale)
    {
        var sorted =  G.I.Data.enemyContainer
            .List
            .Where(x => !x.excludeFromLevelGeneration)
            .OrderBy(e => e.DamagePerHit)
            .ToList();

        var maxStrong = (int)Mathf.Lerp(1, 4, scale);
        var strongList = sorted.Skip(sorted.Count / 2).PickRandom(maxStrong);

        var weakList = sorted.Except(strongList).ToList();
        var maxWeak = (int)Mathf.Lerp(sorted.Count/2f, weakList.Count, scale);

        var weakLookup = weakList.Take(maxWeak).ToList();
        
        var currentWeakNum = (int)Mathf.Lerp(3, weakLookup.Count, scale);
        var weakFinal = weakLookup.PickRandom(currentWeakNum);
        
        var finalEnemyList = weakFinal.Concat(strongList).ToList();
        
        var final = GetLevelEnemyInfos(scale, finalEnemyList);
        return final;
    }

    private static List<LevelEnemyInfo> GetLevelEnemyInfos(float scale, List<EnemyData> finalEnemyList)
    {
        // Нормализация значений для рассчёта шансов
        float minHp = finalEnemyList.Min(e => e.Health);
        float maxHp = finalEnemyList.Max(e => e.Health);
        float hpRange = Mathf.Max(maxHp - minHp, Mathf.Epsilon);

        float minDmg = finalEnemyList.Min(e => e.DamagePerHit);
        float maxDmg = finalEnemyList.Max(e => e.DamagePerHit);
        float dmgRange = Mathf.Max(maxDmg - minDmg, Mathf.Epsilon);

        float baseChance = 5f;
        float maxChance = 70f;

        return finalEnemyList.Select(enemy => new LevelEnemyInfo
        {
            enemyId = enemy.EnemyId,
            spawnChance = GetChance(enemy, minHp, hpRange, minDmg, dmgRange, scale, baseChance, maxChance),
            spawnRate = GetSpawnRate(scale, enemy)
        }).ToList();
    }

    private static int GetDynamicSpawnRate(float scale)
    {
        var min = GetDynamicScaledSpawnRate(scale, 30, 60);
        var max = GetDynamicScaledSpawnRate(scale, 40, 80);

        max = Mathf.Max(max, min + 1);
        var final = Random.Range(min, max + 1);
        return final;
    }

    private static int GetDynamicScaledSpawnRate(float scale, int min, int max)
    {
        var spawnRate = Mathf.RoundToInt(Mathf.Lerp(min, max, scale));
        return Random.Range(min, spawnRate + 1);
    }
}