public class PoisonRoundModification : IRoundModification
{
    public LevelModifierType ModifierType => LevelModifierType.Poison;

    public void OnRoundStart(RoundManager roundManager)
    {
    }

    public void OnRoundUpdate(RoundManager roundManager)
    {
        var diff = UnityEngine.Time.deltaTime * roundManager._currentLevelConfig.poisonDamageSpeed;
        G.I.HpManager.Reduce(diff, doBlink: false, ignoreShield: true);
    }

    public void OnRoundEnd(RoundManager roundManager)
    {
    }
}