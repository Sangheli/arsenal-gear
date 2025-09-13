public class LevelEndFactory : ILevelTargetEnd
{
    public float CurrentValue { get; }
    public string Localization { get; }

    public int GetEnergy()
    {
        return 0;
    }

    public void Add(LevelObjectiveType levelObjectiveType, float value)
    {
        // throw new System.NotImplementedException();
    }

    public void OnEnd()
    {
        // throw new System.NotImplementedException();
    }

    public void ResetValue(LevelConfig currentLevelConfig)
    {
        // throw new System.NotImplementedException();
    }

    public bool IsCompleted()
    {
        return false;
        // throw new System.NotImplementedException();
    }

    public float GetLevelPercent()
    {
        return 0;
        // throw new System.NotImplementedException();
    }
}