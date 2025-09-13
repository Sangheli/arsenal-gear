public interface IRoundModification
{
    LevelModifierType ModifierType { get; }
    void OnRoundStart(RoundManager roundManager);
    void OnRoundUpdate(RoundManager roundManager);
    void OnRoundEnd(RoundManager roundManager);
}