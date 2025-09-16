using System;
using YG;

namespace SangheliCore.Adv
{
    public class YAAdvertisement : IAdvertisement
    {
        public event Action OnInterstitialFinished;
        public event Action<bool> OnRewardedAdFinished;
        public void ShowInterstitial()
        {
            YG2.InterstitialAdvShow();
        }

        public void ShowRewardedAd(Action<bool> rewardedCallback)
        {
            throw new NotImplementedException();
        }

        public bool IsInterstitialReady()
        {
            throw new NotImplementedException();
        }

        public bool IsRewardedAdReady()
        {
            throw new NotImplementedException();
        }
    }
}