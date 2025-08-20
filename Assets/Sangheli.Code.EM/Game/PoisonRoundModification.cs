public class PoisonRoundModification : IRoundModification
{
    public LevelModifierType ModifierType => LevelModifierType.Poison;

    public void OnRoundStart(RoundManager roundManager)
    {
    }

    public void OnRoundUpdate(RoundManager roundManager)
    {
        var diff = UnityEngine.Time.deltaTime * 1;
        diff *= 2/3f;
        G.I.HpManager.Reduce(diff, doBlink: false, ignoreShield: true, doAnimatedUnfill: false);
    }

    public void OnRoundEnd(RoundManager roundManager)
    {
    }
}