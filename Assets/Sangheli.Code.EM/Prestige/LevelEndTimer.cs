using UnityEngine;

public class LevelEndTimer : ILevelTargetEnd
{
    public string Localization => "level_complete";
    public int GetEnergy() => 0;
    
    public float CurrentValue { get; private set; }
    private LevelConfig _currentLevelConfig;
    public void OnEnd()
    {
        G.ChangeLevel(1);
        G.I.StopRun(this, EndRoundFadeEnum.RoundWin);
    }

    public void  ResetValue(LevelConfig currentLevelConfig)
    {
        CurrentValue = 0;
        _currentLevelConfig = currentLevelConfig;
    }

    public void Add(LevelObjectiveType levelObjectiveType, float value)
    {
        if(levelObjectiveType != LevelObjectiveType.Time)
            return;
        
        CurrentValue += value;
    }

    public bool IsCompleted() => CurrentValue >=  _currentLevelConfig.roundTime;
    public float GetLevelPercent() =>  _currentLevelConfig.roundTime <= 0f ? 0f : Mathf.Clamp01(CurrentValue /  _currentLevelConfig.roundTime);
}