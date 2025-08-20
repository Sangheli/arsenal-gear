using UnityEngine;

public class LevelEndBoss : ILevelTargetEnd
{
    public string Localization => "win_text";
    public LevelTargetValue Data;
    public float CurrentValue { get; private set; }
    private LevelConfig _levelConfig;
    
    public void OnEnd()
    {
        var level = G.I.SaveProvider.Get().level;
        Color bossColor = Data.LevelGradientColor.GetLevelColor(level + 1);
        float bossHp = _levelConfig?.bossHp ?? 1000f;
        float scale = 9f;
        float damage = _levelConfig?.bossAttack ?? 1;
        Data.Active = false;
        G.I.tutorial.OnEndRound(); // Скрываем туториал при появлении босса
        
        // Set boss active flag in RoundManager
        Data.TargetView.gameObject.SetActive(false);
        G.I.roundManager.SetBossActive(true);
        G.I.bossManager.SpawnBoss(bossHp, bossColor, scale,damage, CompletePrestigeAfterBoss);
    }

    private void CompletePrestigeAfterBoss()
    {
        G.I.roundManager.SetBossActive(false);
        G.ChangeLevel(1);
        G.I.StopRun(this, EndRoundFadeEnum.RoundWin);
    }
    
    public int GetEnergy() => _levelConfig?.levelReward ?? 100;

    public void Add(LevelObjectiveType levelObjectiveType, float value)
    {
        if(levelObjectiveType != LevelObjectiveType.Boss)
            return;
        
        CurrentValue += value;
    }
    
    public void ResetValue(LevelConfig currentLevelConfig)
    {
        CurrentValue = 0;
        _levelConfig = currentLevelConfig;
    }

    public bool IsCompleted() => CurrentValue >=  _levelConfig.roundTime;
    public float GetLevelPercent() =>  _levelConfig.roundTime <= 0f ? 0f : Mathf.Clamp01(CurrentValue /  _levelConfig.roundTime);
}