using System;
using System.Collections.Generic;
using System.Linq;

public class LevelDataConfigRuntime
{
    private readonly Dictionary<int, LevelConfig> _levels = new();

    public LevelDataConfigRuntime(LevelDataConfigSO configSO)
    {
        if (configSO != null)
        {
            foreach (var level in configSO.Levels)
                _levels[level.levelId] = level;
        }
        
        for (var i = 0; i < G.I.maxLevel; i++)
        {
            if (_levels.ContainsKey(i)) 
                continue;
            
            var newLevel = GenerateRandomLevel();
            newLevel.levelId = i;
            _levels[i] = newLevel;
        }
    }

    private static LevelConfig GenerateRandomLevel() =>
        new()
        {
            enemies = GenerateEnemyes(),
            loseHpDuringRound = true,
            showBossAfterTimer = true,
            checkNextRoundFromSave = false,
            showGoToUpgradesButton = true
        };

    private static List<LevelEnemyInfo> GenerateEnemyes()
    {
        var allEnemies = G.I.Data.enemyContainer.List;
        var numEnemies = UnityEngine.Random.Range(1, Math.Min(4, allEnemies.Count + 1));
        var chosenIndices = new HashSet<int>();

        while (chosenIndices.Count < numEnemies)
        {
            var idx = UnityEngine.Random.Range(0, allEnemies.Count);
            chosenIndices.Add(idx);
        }

        return chosenIndices
            .Select(x => allEnemies[x])
            .Select(GenerateEnemy)
            .ToList();
    }

    private static LevelEnemyInfo GenerateEnemy(EnemyData enemy)
    {
        return new LevelEnemyInfo
        {
            enemyId = enemy.EnemyId,
            spawnChance = UnityEngine.Random.Range(30f, 101f), // 30-100
            spawnRate = UnityEngine.Random.Range(20f, 81f) // 20-80 per minute
        };
    }

    public LevelConfig Get(int levelId)
    {
        _levels.TryGetValue(levelId, out var config);
        return config;
    }

    public int Count() => _levels.Count;
}