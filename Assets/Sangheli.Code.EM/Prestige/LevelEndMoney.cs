using UnityEngine;

public class LevelEndMoney : ILevelTargetEnd
{
    public string Localization => "level_complete";
    public int GetEnergy() => 0;

    private int _value;
    public float Value => _value;

    private LevelConfig _currentLevelConfig;
    
    public void OnEnd()
    {
        G.ChangeLevel(1);
        G.I.StopRun(this, EndRoundFadeEnum.RoundWin);
    }

    public void  ResetValue(LevelConfig currentLevelConfig, float getTime)
    {
        _currentLevelConfig = currentLevelConfig;
        _value = 0;
    }

    public void Add(LevelObjectiveType levelObjectiveType, float value)
    {
        if(levelObjectiveType != LevelObjectiveType.CoinCount)
            return;
        
        _value += (int)value;
    }

    public bool IsCompleted() => _value >= _currentLevelConfig.objectiveValue;
    public bool IsBossTimerOver() => false;
    public float GetLevelTimePercent() => Mathf.Clamp01(_value / (float) _currentLevelConfig.objectiveValue);
    public float GetPrestigeTime() => throw new System.NotImplementedException();
    public float GetBossDistance() => throw new System.NotImplementedException();
}