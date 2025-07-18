using System;
using System.Collections.Generic;
using System.Linq;

public class LevelDataConfigRuntime
{
    private readonly Dictionary<int, LevelConfig> _levels = new();

    public LevelDataConfigRuntime(LevelDataConfigSO configSO)
    {
        var save = G.I.SaveProvider.Get();
        if (configSO != null)
        {
            for (var index = 0; index < configSO.Levels.Count; index++) 
            {
                var level = configSO.Levels[index];
                if (level.GenerateRandomEnemy)
                {
                    var savedEnemies = save.GetRandomLevelEnemies(index);
                    if (savedEnemies != null)
                    {
                        level.enemies = savedEnemies;
                    }
                    else
                    {
                        level.enemies = GenerateEnemyes();
                        save.SetRandomLevelEnemies(index, level.enemies);
                    }
                }
                _levels[index] = level;
            }
        }
        
        for (var i = 0; i < G.I.maxLevel; i++)
        {
            if (_levels.ContainsKey(i)) 
                continue;
            
            var newLevel = GenerateRandomLevel(i, save);
            _levels[i] = newLevel;
        }
    }

    private static LevelConfig GenerateRandomLevel(int levelIndex, DataSave save)
    {
        var enemies = save.GetRandomLevelEnemies(levelIndex);
        if (enemies == null)
        {
            enemies = GenerateEnemyes();
            save.SetRandomLevelEnemies(levelIndex, enemies);
        }
        return new LevelConfig
        {
            enemies = enemies,
            loseHpDuringRound = true,
            showBossAfterTimer = true,
            checkNextRoundFromSave = false,
            showGoToUpgradesButton = true,
            preSpawnEnemyCount = 10,
            GenerateRandomEnemy = true
        };
    }

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