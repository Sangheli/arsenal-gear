public interface ILevelTargetEnd
{
    float CurrentValue { get; }
    string Localization { get; }
    int GetEnergy();
    void Add(LevelObjectiveType levelObjectiveType, float value);
    void OnEnd();
    void ResetValue(LevelConfig currentLevelConfig);
    bool IsCompleted();
    public float GetLevelPercent();
}