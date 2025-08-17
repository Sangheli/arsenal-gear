using UnityEngine;

public class LevelEndBoss : ILevelTargetEnd
{
    public string Localization => "win_text";
    public LevelTargetValue Data;
    public float Value { get; private set; }
    public float PrestigeTime;
    public float BossDistance;
    private LevelConfig _currentLevelConfig;
    private LevelConfig _levelConfig;
    
    public void OnEnd()
    {
        var level = G.I.SaveProvider.Get().level;
        _levelConfig = G.I.LevelDataConfig.Count() > level ? G.I.LevelDataConfig.Get(level) : null;
        Color bossColor = Data.LevelGradientColor.GetLevelColor(level + 1);
        float bossHp = _levelConfig?.bossHp ?? 1000f;
        float scale = 9f;
        float damage = _levelConfig?.bossAttack ?? 1;
        Data.Active = false;
        G.I.tutorial.OnEndRound(); // Скрываем туториал при появлении босса
        
        // Set boss active flag in RoundManager
        G.I.roundManager.SetBossActive(true);
        G.I.bossManager.SpawnBoss(bossHp, bossColor, scale,damage, CompletePrestigeAfterBoss);
    }

    private void CompletePrestigeAfterBoss()
    {
        G.I.roundManager.SetBossActive(false);
        G.ChangeLevel(1);
        G.I.StopRun(this, EndRoundFadeEnum.RoundWin);
    }
    
    public int GetEnergy() => _levelConfig?.bossReward ?? 100;

    public void Add(LevelObjectiveType levelObjectiveType, float value)
    {
        if(levelObjectiveType != LevelObjectiveType.Boss)
            return;
        
        Value += value;
    }
    
    public void ResetValue(LevelConfig currentLevelConfig, float getTime)
    {
        _currentLevelConfig = currentLevelConfig;
        Value = 0;
        PrestigeTime = getTime;
        BossDistance = currentLevelConfig.objectiveValue;
    }

    public bool IsCompleted() => Value >= PrestigeTime;
    public bool IsBossTimerOver() => _currentLevelConfig.objectiveType == LevelObjectiveType.Boss && Value >= PrestigeTime;
    public float GetLevelTimePercent() => PrestigeTime <= 0f ? 0f : Mathf.Clamp01(Value / PrestigeTime);
    public float GetPrestigeTime() => PrestigeTime;
    public float GetBossDistance() => BossDistance;
}