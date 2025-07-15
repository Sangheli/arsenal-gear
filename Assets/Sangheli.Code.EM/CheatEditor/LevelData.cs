using System;
using System.Collections.Generic;

[Serializable]
public class LevelEnemyInfo
{
    public int enemyId;
    public float spawnChance; // Вероятность появления (0..100)
    public float spawnRate;   // Спавнрейт (врагов в минуту)
}

[Serializable]
public class LevelConfig
{
    public int levelId;
    public string LevelStringEntry;
    public List<LevelEnemyInfo> enemies = new();
}