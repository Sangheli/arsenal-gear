using YG;

public class ContinueManagerYandex : ContinueManagerSimple
{
    public override void OnContinue()
    {
        YG2.SkipNextInterAdCall();
        YG2.RewardedAdvShow("continue", base.OnContinue);
    }
}