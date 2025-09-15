using UnityEngine;

public class LevelEndFactory : ILevelTargetEnd
{
    public float CurrentValue => _value;
    public string Localization => "level_complete";

    public int GetEnergy()
    {
        return _currentLevelConfig.levelReward;
    }
    
    private LevelConfig _currentLevelConfig;
    private int _value;

    public void Add(LevelObjectiveType levelObjectiveType, float value)
    {
        if (levelObjectiveType != LevelObjectiveType.Factory)
            return;

        _value += (int)value;
    }
    
    public void OnEnd()
    {
        G.ChangeLevel(1);
        G.I.StopRun(this, EndRoundFadeEnum.RoundWin);
    }

    public void ResetValue(LevelConfig currentLevelConfig)
    {
        _currentLevelConfig = currentLevelConfig;
        _value = 0;
    }

    public bool IsCompleted()
    {
        return _value >= _currentLevelConfig.objectiveValue;
    }

    public float GetLevelPercent()
    {
        return Mathf.Clamp01(_value / (float)_currentLevelConfig.objectiveValue);
    }
}