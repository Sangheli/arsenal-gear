using System.Collections.Generic;
using UnityEngine;

public class LevelTargetValue
{
    public bool Active;
    public readonly LevelGradientColor LevelGradientColor;
    
    private LevelConfig _currentLevelConfig;
    public readonly LevelTargetBar TargetView;
    public ILevelTargetEnd End;
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
        
        TargetView = targetView;
        LevelGradientColor = levelGradientColor;
    }

    public void Init(LevelConfig currentLevelConfig)
    {
        TargetView.gameObject.SetActive(true);
        Active = true;
        _currentLevelConfig = currentLevelConfig;
        End = GetLevelTargetEnd(currentLevelConfig.objectiveType);
        End.ResetValue(currentLevelConfig);
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
        if (End.IsCompleted()) 
            return;
        
        End.Add(levelObjectiveType, value);
        TargetView.UpdateView();

        if (!End.IsCompleted()) 
            return;
        
        Active = false;
        End.OnEnd();
    }

    public void ToggleActive(bool state) => Active = state;
}