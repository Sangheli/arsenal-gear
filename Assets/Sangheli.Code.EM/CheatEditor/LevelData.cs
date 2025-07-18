using System;

[Serializable]
public class LevelEnemyInfo
{
    public int enemyId;
    public float spawnChance; // Вероятность появления (0..100)
    public float spawnRate;   // Спавнрейт (врагов в минуту)
}

[Serializable]
public class LevelConfigItem
{
    public int levelId;
    public LevelConfig config;
}