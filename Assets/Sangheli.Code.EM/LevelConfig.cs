using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelConfig", menuName = "Game/Level Config")]
public class LevelConfig : ScriptableObject
{
    public string LevelStringEntry;
    public List<LevelEnemyInfo> enemies = new();
    public bool loseHpDuringRound = true; // Тратить ли жизнь игрока во время раунда
    public bool showBossAfterTimer = true; // Показывать ли босса после таймера (true) или сразу засчитывать победу (false)
    public bool checkNextRoundFromSave = false; // Показывать ли кнопку перехода на следующий уровень
    public bool showGoToUpgradesButton = true; // Показывать ли кнопку перехода на экран апгрейдов на экране окончания раунда
}