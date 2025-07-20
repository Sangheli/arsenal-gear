using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class LevelConfig
{
    public int levelId;
    
    [Space]
    public string LevelStringEntry;
    
    [Space]
    public bool loseHpDuringRound = true; // Тратить ли жизнь игрока во время раунда
    public bool showBossAfterTimer = true; // Показывать ли босса после таймера (true) или сразу засчитывать победу (false)
    public bool checkNextRoundFromSave = false; // Показывать ли кнопку перехода на следующий уровень
    public bool showGoToUpgradesButton = true; // Показывать ли кнопку перехода на экран апгрейдов на экране окончания раунда
    
    [Space]
    public int preSpawnEnemyCount = 10; // Количество врагов для предспавна в начале раунда

    [Space]
    public List<LevelEnemyInfo> enemies = new();
}