using System;

[Serializable]
public class LevelEnemyInfo
{
    public int enemyId;
    public int spawnChance; // Вероятность появления (0..100)
    public int spawnRate;   // Спавнрейт (врагов в минуту)
}