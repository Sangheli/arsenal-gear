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
    public float bossAttack = 5;
    
    [Header("Reward")]
    public int levelReward = 100;
    
    [Space]
    public bool showHpBar = true; // Показывать или нет шкалу жизни
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
    public float poisonDamageSpeed = 0f; // Скорость уменьшения жизни в секунду при ядовом модификаторе

    [Space]
    public List<LevelModifierType> levelModifiers = new();

    [Space]
    public List<LevelEnemyInfo> enemies = new();
    
    [Space]
    public bool isSafeLevel = false; // Уровень, на котором игрок не может умереть (здоровье не может упасть ниже 5%)
    
    [Space]
    public int MaxAttackingEnemies = 3;
    public int MaxEnemyOnScreen = 10;

    public override string ToString()
    {
        return $"{nameof(levelId)}: {levelId}\n {nameof(LevelStringEntry)}: {LevelStringEntry}\n {nameof(objectiveType)}: {objectiveType}\n {nameof(objectiveValue)}: {objectiveValue}\n {nameof(roundTime)}: {roundTime}\n {nameof(bossHp)}: {bossHp}\n {nameof(bossAttack)}: {bossAttack}\n {nameof(levelReward)}: {levelReward}\n {nameof(showHpBar)}: {showHpBar}\n {nameof(checkNextRoundFromSave)}: {checkNextRoundFromSave}\n {nameof(showGoToUpgradesButton)}: {showGoToUpgradesButton}\n {nameof(showTerminateButton)}: {showTerminateButton}\n {nameof(useWinColorOnFail)}: {useWinColorOnFail}\n {nameof(playerAutoAttack)}: {playerAutoAttack}\n {nameof(bossEnemySpawnRateMultiplier)}: {bossEnemySpawnRateMultiplier}\n {nameof(preSpawnEnemyCount)}: {preSpawnEnemyCount}\n {nameof(poisonDamageSpeed)}: {poisonDamageSpeed}\n {nameof(levelModifiers)}: {levelModifiers}\n {nameof(enemies)}: {enemies}\n {nameof(isSafeLevel)}: {isSafeLevel}\n {nameof(MaxAttackingEnemies)}: {MaxAttackingEnemies}\n {nameof(MaxEnemyOnScreen)}: {MaxEnemyOnScreen}";
    }
}