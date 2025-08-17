using UnityEngine;
using UnityEngine.UI;

public class LevelTargetBar : MonoBehaviour
{
    public Text progressText;
    public Image objectiveIcon;
    private LevelConfig _currentLevelConfig;
    private LevelObjectiveVisualConfig.ObjectiveVisualData _visualData;
    private LevelTargetValue _levelTargetValue;
    
    public void Init(LevelConfig currentLevelConfig, LevelTargetValue levelTargetValue)
    {
        _currentLevelConfig = currentLevelConfig;
        _levelTargetValue = levelTargetValue;
        _visualData = G.I.Data.objectiveVisualConfig.Get(_currentLevelConfig.objectiveType);
    }
    
    public void UpdateView()
    {
        objectiveIcon.sprite = _visualData?.sprite;
        objectiveIcon.gameObject.SetActive(_visualData != null);

        switch (_currentLevelConfig.objectiveType)
        {
            case LevelObjectiveType.CoinCount:
                progressText.text = $"{_levelTargetValue.LevelTargetEnd.Value}/{_currentLevelConfig.objectiveValue}";
                break;
            case LevelObjectiveType.Boss:
                // Calculate remaining distance as (1 - timeProgress) * bossDistance
                float timeProgress = _levelTargetValue.LevelTargetEnd.GetPrestigeTime() > 0 ? _levelTargetValue.LevelTargetEnd.Value / _levelTargetValue.LevelTargetEnd.GetPrestigeTime() : 0;
                float remainingDistance = (1 - timeProgress) * _levelTargetValue.LevelTargetEnd.GetBossDistance();
                progressText.text = $"{Mathf.CeilToInt(remainingDistance)}";
                break;
            case LevelObjectiveType.Time:
                float percent = _levelTargetValue.LevelTargetEnd.GetPrestigeTime() > 0 ? _levelTargetValue.LevelTargetEnd.Value / _levelTargetValue.LevelTargetEnd.GetPrestigeTime() : 0;
                progressText.text = $"{Mathf.RoundToInt(percent * 100)}%";
                break;
        }
    }
}