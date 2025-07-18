using System;

[Serializable]
public class LevelEnemyInfo
{
    public int enemyId;
    public float spawnChance; // Вероятность появления (0..100)
    public float spawnRate;   // Спавнрейт (врагов в минуту)
}