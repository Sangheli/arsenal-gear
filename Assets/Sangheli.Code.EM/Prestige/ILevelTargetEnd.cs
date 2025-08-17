public interface ILevelTargetEnd
{
    float Value { get; }
    string Localization { get; }
    int GetEnergy();
    void Add(LevelObjectiveType levelObjectiveType, float value);
    void OnEnd();
    void ResetValue(LevelConfig currentLevelConfig, float getTime);
    bool IsCompleted();
    public bool IsBossTimerOver();
    public float GetLevelTimePercent();
    float GetPrestigeTime();
    float GetBossDistance();
}