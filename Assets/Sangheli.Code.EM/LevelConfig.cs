using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class LevelConfig
{
    public int levelId;
    
    [Space]
    public string LevelStringEntry;
    
    [Header("Round Settings")]
    public float roundTime = 30f; // Время раунда в секундах
    
    [Header("Boss Settings")]
    public int bossHp = 60;
    public int bossReward = 100;
    public float bossAttack = 5;
    
    [Space]
    public bool loseHpDuringRound = true; // Тратить ли жизнь игрока во время раунда
    public bool showBossAfterTimer = true; // Показывать ли босса после таймера (true) или сразу засчитывать победу (false)
    public bool checkNextRoundFromSave = false; // Показывать ли кнопку перехода на следующий уровень
    public bool showGoToUpgradesButton = true; // Показывать ли кнопку перехода на экран апгрейдов на экране окончания раунда
    public bool showTerminateButton = true; // Показывать ли кнопку прерывания уровня во время игры
    
    [Space]
    public bool autoCollectAllCoins = false; // Автоматически собирать все монеты в конце раунда
    
    [Space]
    public int preSpawnEnemyCount = 10; // Количество врагов для предспавна в начале раунда

    [Space]
    public List<LevelEnemyInfo> enemies = new();
}