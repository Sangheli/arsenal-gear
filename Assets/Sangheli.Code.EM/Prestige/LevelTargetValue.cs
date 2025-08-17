using System.Collections.Generic;
using UnityEngine;

public class LevelTargetValue
{
    public bool Active;
    public readonly LevelGradientColor LevelGradientColor;
    
    private LevelConfig _currentLevelConfig;
    private readonly LevelTargetBar _targetView;
    public ILevelTargetEnd LevelTargetEnd;
    private readonly Dictionary<LevelObjectiveType, ILevelTargetEnd> _objectiveEnd;
    
    public LevelTargetValue(LevelTargetBar targetView,LevelGradientColor levelGradientColor)
    {
        _objectiveEnd = new Dictionary<LevelObjectiveType, ILevelTargetEnd>
        {
            { LevelObjectiveType.Boss, new LevelEndBoss {Data = this}},
            { LevelObjectiveType.CoinCount, new LevelEndMoney()},
            { LevelObjectiveType.Time, new LevelEndTimer()},
            { LevelObjectiveType.EnemyCount, new LevelEndEnemy()},
        };
        
        _targetView = targetView;
        LevelGradientColor = levelGradientColor;
    }

    public void Init(LevelConfig currentLevelConfig, float getTime)
    {
        Active = true;
        _currentLevelConfig = currentLevelConfig;
        LevelTargetEnd = GetLevelTargetEnd(currentLevelConfig.objectiveType);
        LevelTargetEnd.ResetValue(currentLevelConfig, getTime);
    }

    private ILevelTargetEnd GetLevelTargetEnd(LevelObjectiveType type)
    {
        _objectiveEnd.TryGetValue(type, out var value);
        return value;
    }

    public void AddTimeValue()
    {
        if (_currentLevelConfig.objectiveType is not (LevelObjectiveType.Boss or LevelObjectiveType.Time)) 
            return;
        
        AddValue(_currentLevelConfig.objectiveType, Time.deltaTime);
    }

    public void AddValue(LevelObjectiveType levelObjectiveType, float value)
    {
        if (LevelTargetEnd.IsCompleted()) 
            return;
        
        LevelTargetEnd.Add(levelObjectiveType, value);
        _targetView.UpdateView();

        if (!LevelTargetEnd.IsCompleted()) 
            return;
        
        Active = false;
        LevelTargetEnd.OnEnd();
    }

    public void ToggleActive(bool state) => Active = state;
}