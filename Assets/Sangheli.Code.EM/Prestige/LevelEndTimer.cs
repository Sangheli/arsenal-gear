using UnityEngine;

public class LevelEndTimer : ILevelTargetEnd
{
    public string Localization => "level_complete";
    public int GetEnergy() => 0;
    public float Value { get; private set; }
    private float _prestigeTime;
    
    public void OnEnd()
    {
        G.ChangeLevel(1);
        G.I.StopRun(this, EndRoundFadeEnum.RoundWin);
    }

    public void  ResetValue(LevelConfig currentLevelConfig, float getTime)
    {
        Value = 0;
        _prestigeTime = getTime;
    }

    public void Add(LevelObjectiveType levelObjectiveType, float value)
    {
        if(levelObjectiveType != LevelObjectiveType.Time)
            return;
        
        Value += value;
    }

    public bool IsCompleted() => Value >= _prestigeTime;
    public bool IsBossTimerOver() => false;
    public float GetLevelTimePercent() => _prestigeTime <= 0f ? 0f : Mathf.Clamp01(Value / _prestigeTime);
    public float GetPrestigeTime() => _prestigeTime;
    public float GetBossDistance() => throw new System.NotImplementedException();
}