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
        objectiveIcon.color = _visualData.color;
        objectiveIcon.gameObject.SetActive(_visualData != null);

        switch (_currentLevelConfig.objectiveType)
        {
            case LevelObjectiveType.CoinCount:
            case LevelObjectiveType.EnemyCount:
                progressText.text = $"{NumberFormatter.Format(_levelTargetValue.End.CurrentValue, 0)}/{NumberFormatter.Format(_currentLevelConfig.objectiveValue, 0)}";
                break;
            case LevelObjectiveType.Boss:
                var distance = (1 - GetTimePercent()) * _currentLevelConfig.objectiveValue;
                progressText.text = $"{Mathf.CeilToInt(distance)}";
                break;
            case LevelObjectiveType.Time:
                progressText.text = $"{Mathf.RoundToInt(GetTimePercent() * 100)}%";
                break;
        }
    }

    private float GetTimePercent() => 
        _currentLevelConfig.roundTime > 0 ? _levelTargetValue.End.CurrentValue /  _currentLevelConfig.roundTime : 0;
}