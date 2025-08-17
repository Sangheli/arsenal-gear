using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class LevelConfig
{
    public int levelId;
    
    [Space]
    public string LevelStringEntry;
    
    [Header("Level Objective")]
    public LevelObjectiveType objectiveType = LevelObjectiveType.CoinCount;
    public int objectiveValue = 25;
    
    [Header("Round Settings")]
    public float roundTime = 30f; // Время раунда в секундах
    
    [Header("Boss Settings")]
    public int bossHp = 60;
    public int bossReward = 100;
    public float bossAttack = 5;
    
    [Space]
    public bool loseHpDuringRound = true; // Тратить ли жизнь игрока во время раунда
    public bool checkNextRoundFromSave = false; // Показывать ли кнопку перехода на следующий уровень
    public bool showGoToUpgradesButton = true; // Показывать ли кнопку перехода на экран апгрейдов на экране окончания раунда
    public bool showTerminateButton = true; // Показывать ли кнопку прерывания уровня во время игры
    
    [Space]
    public bool useWinColorOnFail = false; // Использовать цвет победы даже при поражение
    
    [Space]
    public bool playerAutoAttack = true; // Отключить автоатаку игрока
    
    [Space]
    public float bossEnemySpawnRateMultiplier = 5; // Множитель уменьшения скорости спавна врагов при появлении босса
    
    [Space]
    public int preSpawnEnemyCount = 10; // Количество врагов для предспавна в начале раунда

    [Space]
    public List<LevelEnemyInfo> enemies = new();
}