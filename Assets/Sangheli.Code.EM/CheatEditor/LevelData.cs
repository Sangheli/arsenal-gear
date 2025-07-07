using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class LevelEnemyInfo
{
    public int enemyId;
    public float spawnChance; // Вероятность появления (0..1)
    public float spawnRate;   // Спавнрейт (врагов в минуту)
}

[Serializable]
public class LevelConfig
{
    public int levelId;
    public List<LevelEnemyInfo> enemies = new();
}

public static class LevelData
{
    public static readonly List<LevelConfig> Levels;

    static LevelData()
    {
        Levels = new List<LevelConfig>();
        var random = new System.Random(42); // фиксированный seed для повторяемости
        int enemyTypes = 7; // Предположим, что есть 5 типов врагов с id 0-4
        for (int i = 0; i <= 50; i++)
        {
            var config = new LevelConfig { levelId = i };
            int enemiesInLevel = random.Next(1, enemyTypes + 1);
            var used = new HashSet<int>();
            for (int j = 0; j < enemiesInLevel; j++)
            {
                int enemyId;
                do { enemyId = random.Next(0, enemyTypes); } while (!used.Add(enemyId));
                config.enemies.Add(new LevelEnemyInfo
                {
                    enemyId = enemyId,
                    spawnChance = (float)random.NextDouble(),
                    spawnRate = (float)(random.NextDouble() * 10 + 1)
                });
            }
            Levels.Add(config);
        }
    }
} 