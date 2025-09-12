using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using UnityEngine;
using Random = UnityEngine.Random;

public class LevelDataConfigRuntime
{
    private readonly Dictionary<int, LevelConfig> _levels = new();

    public const int SkipTutorialLevels = 5;
    
    private const double MinRewardBoss = 100;
    private static double MaxRewardBoss = 2000000;
    private const  double Exponent = 0.3975f;
    private float coef = 13.28f;
    
    private const int tierStep = 5;

    public LevelDataConfigRuntime()
    {
        var totalPrice = G.I.Data.ParamData.GetTotalPrice();
        MaxRewardBoss = (uint)(totalPrice / coef);
        
        for (var i = 0; i < G.I.maxLevel; i++)
        {
            if (_levels.ContainsKey(i)) continue;
            if (TryLoadLevel(i)) continue;
            
            // var level = GenerateRandomLevel(i);
            // _levels[i] = level;
            // SaveLevelToJson(i, level);
        }
    }
    
    private bool TryLoadLevel(int i)
    {
        // if(i>4)
        //     return false;
        var loadedLevel = LoadLevelFromJson(i);
        if (loadedLevel == null) return false;
        _levels[i] = loadedLevel;
        return true;
    }

    private LevelConfig GenerateRandomLevel(int i)
    {
        var scale = GetScaleValue(i);
        var objectiveType = GetObjectiveType(i);
        var playerDps = GetPlayerDps(scale);
        var bossHp = CalculateBossHp(scale, playerDps);
        
        // First few levels are safe levels (tutorial levels)
        bool isSafeLevel = i < SkipTutorialLevels;

        return new LevelConfig
        {
            levelId = i,
            enemies = GenerateEnemies(scale, i),
            showHpBar = true,
            checkNextRoundFromSave = false,
            showGoToUpgradesButton = true,
            showTerminateButton = true,
            preSpawnEnemyCount = 10,
            roundTime = 23.5f,
            bossHp = bossHp,
            levelReward =  GetLevelReward(objectiveType, scale, i),
            bossAttack = CalculateBossAttack(scale, playerDps , bossHp),
            bossEnemySpawnRateMultiplier = 5, // Default value for boss enemy spawn rate multiplier
            useWinColorOnFail = false, // By default, use normal fail color on defeat
            playerAutoAttack = true, // By default, player auto-attack is enabled
            objectiveType = objectiveType,
            objectiveValue = GetObjectiveValue(objectiveType, scale),
            levelModifiers = GenerateRandomModifiers(scale), // Generate random modifiers for the level
            isSafeLevel = isSafeLevel, // Mark tutorial levels as safe
            MaxAttackingEnemies = GetMaxAttackNum(scale),
        };
    }

    private int GetMaxAttackNum(float scale) => Mathf.RoundToInt(Mathf.Lerp(3, 15, scale));

    private static List<LevelModifierType> GenerateRandomModifiers(float scale)
    {
        return new List<LevelModifierType>()
        {
            // LevelModifierType.Poison,
        };
    }

    public LevelConfig Get(int levelId, bool overrideNew)
    {
        _levels.TryGetValue(levelId, out var config);
        return (overrideNew && levelId >= SkipTutorialLevels) || config == null
            ? _levels[levelId] = GenerateRandomLevel(levelId)
            : config;
    }

    private static string GetLevelJsonPath(int levelIndex)
    {
#if UNITY_EDITOR
        var dir = Path.Combine(Application.dataPath, "Levels");
#else
        var dir = Path.Combine(Application.persistentDataPath, "Levels");
#endif
        // if (!Directory.Exists(dir))
        //     Directory.CreateDirectory(dir);
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
                int coinMax = Random.Range(5000, 6000);
                return Mathf.RoundToInt(Mathf.Lerp(coinMin, coinMax, t));
            case LevelObjectiveType.EnemyCount:
                int min = Random.Range(15, 25);
                int max = Random.Range(120, 150);
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
        float minDuration = 20f;
        float maxDuration = 25f;
        float fightDuration = Mathf.Lerp(minDuration, maxDuration, t);

        // Calculate boss health based on player DPS and desired fight duration
        int bossHp = Mathf.RoundToInt(playerDps * fightDuration);
        return bossHp;
    }

    private static float GetPlayerDps(float scale)
    {
        var (dps, dpsCrit) = DPSManager.GetPlayerDps();
        
        float minDps = DPSManager.GetPlayerDpsMin();
        float maxDps = dpsCrit;
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

    private static int GetLevelReward(LevelObjectiveType objectiveType, float t, int i)
    {
        if (objectiveType == LevelObjectiveType.Boss)
        {
            var reward = (int)CalculateReward(t, MinRewardBoss, MaxRewardBoss, Exponent);
            return reward;
        }
        else
        {
            return 0;   
        }
    }
    
    static double CalculateReward(double scale, double minReward, double maxReward, double exponent)
    {
        // экспоненциальная прогрессия с контролем кривизны
        double adjustedScale = Math.Pow(scale, exponent);
        return minReward * Math.Pow(maxReward / minReward, adjustedScale);
    }
    
    private List<LevelEnemyInfo> GenerateEnemies(float scale, int i)
    {
        var sorted =  G.I.Data.enemyContainer
            .List
            .Where(x => !x.excludeFromLevelGeneration)
            .ToList();

        var selectedEnemies = GetSelectedEnemies(scale, sorted, i);
        
        // Найдем минимальное и максимальное количество здоровья среди выбранных врагов
        var minHealth = selectedEnemies.Min(e => e.enemyData.GetHP(e.difficult));
        var maxHealth = selectedEnemies.Max(e => e.enemyData.GetHP(e.difficult));
        var healthRange = maxHealth - minHealth;
        
        // Чем больше врагов, тем меньше шанс появления каждого из них
        // Используем коэффициент от 0.5 до 1.0 в зависимости от количества врагов (от 1 до 15)
        var countFactor = Mathf.Lerp(1.0f, 0.5f, (float) (selectedEnemies.Count - 1) / 14);
        countFactor = Mathf.Clamp01(countFactor);

        // Generate LevelEnemyInfo for each selected enemy
        return selectedEnemies.Select(enemy => new LevelEnemyInfo
            {
                enemyId = enemy.enemyData.EnemyId,
                difficult = enemy.difficult,
                // Чем ниже здоровье, тем выше шанс появления
                spawnChance = healthRange > 0 
                    ? Mathf.RoundToInt((85 - 35 * ((enemy.enemyData.GetHP(enemy.difficult) - minHealth) / healthRange)) * countFactor) // От 25 до 85
                    : Mathf.RoundToInt(67 * countFactor), // Среднее значение, если все враги имеют одинаковое здоровье
                // Чем больше врагов, тем ниже частота появления каждого из них
                spawnRate = Mathf.RoundToInt(Random.Range(30, 61) * countFactor) // От 15 до 60
            }).ToList();
    }

    private List<LevelEnemyInfoToGenerate> GetSelectedEnemies(float scale, List<EnemyData> sorted, int i)
    {
        var maxTier = sorted.Max(x => x.Tier);
        var subTier = (i - SkipTutorialLevels) / tierStep;
        
        var maxRank = G.I.Data.enemyDifficultContainer.Length;
        var currentDifficult = Mathf.Min((int)Math.Ceiling(subTier * 1f / maxTier)+1, maxRank);

        var allDifficult = new List<LevelEnemyInfoToGenerate>();
        for (var idx = 0; idx < currentDifficult; idx++) allDifficult.AddRange(GetLevelEnemyInfos(sorted, subTier, idx, maxTier));
        var enemiesToSelect = GetEnemiesToSelect(allDifficult.Count, scale);
        return allDifficult.PickRandom(enemiesToSelect);
    }

    private static List<LevelEnemyInfoToGenerate> GetLevelEnemyInfos(List<EnemyData> sorted, int subTier, int idx, int maxTier)
    {
        var currentTier = subTier - idx * maxTier;
        var allowedTier = currentTier >= maxTier ? maxTier : currentTier;

        var availableEnemies = sorted.Where(e => e.Tier <= allowedTier).ToList();
        return availableEnemies.Select(x => new LevelEnemyInfoToGenerate { enemyData = x, difficult = idx }).ToList();
    }

    private int GetEnemiesToSelect
    (
        int count,
        float scale
    )
    {
        var min = GetDynamicScale(scale, 4, 8);
        var maxEnemies = (int)(count * 0.85f);
        
        var max = GetDynamicScale(scale, count/2f, maxEnemies);
        max = Mathf.Max(max, min + 1);
        var final = Random.Range(min, max + 1);
        
        return Mathf.Min(final, maxEnemies);
    }

    private static int GetDynamicSpawnRate(float scale)
    {
        var min = GetDynamicScale(scale, 30, 60);
        var max = GetDynamicScale(scale, 40, 80);

        max = Mathf.Max(max, min + 1);
        var final = Random.Range(min, max + 1);
        return final;
    }

    private static int GetDynamicScale(float scale, float min, float max)
    {
        var spawnRate = Mathf.RoundToInt(Mathf.Lerp(min, max, scale));
        return (int)Random.Range(min, spawnRate + 1);
    }
    
    private void TestAllSumUpgrades()
    {
        var total = 0f;
        
        var totalPrice = G.I.Data.ParamData.GetTotalPrice();
        var maxReward = totalPrice / coef;
        
        for (var i = 0; i < G.I.maxLevel; i++)
        {
            var objectiveType = GetObjectiveType(i);
            if (objectiveType != LevelObjectiveType.Boss) continue;
            
            var scale = GetScaleValue(i);
            var result = (int)CalculateReward(scale, MinRewardBoss, maxReward, Exponent);
            total += result;
        }
        
        Debug.Log($"[coef][{coef}] tota:____ [{totalPrice}/{total}] [{totalPrice/total}]");
    }
}