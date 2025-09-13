using System;

namespace SangheliCore.Adv
{
    /// <summary>
    /// Interface for advertisement functionality
    /// </summary>
    public interface IAdvertisement
    {
        /// <summary>
        /// Event triggered when an interstitial ad finishes showing
        /// </summary>
        event Action OnInterstitialFinished;
    
        /// <summary>
        /// Event triggered when a rewarded ad finishes showing
        /// </summary>
        event Action<bool> OnRewardedAdFinished;
    
        /// <summary>
        /// Shows an interstitial ad
        /// </summary>
        void ShowInterstitial();
    
        /// <summary>
        /// Shows a rewarded ad
        /// </summary>
        /// <param name="rewardedCallback">Callback with true if reward should be given, false otherwise</param>
        void ShowRewardedAd(Action<bool> rewardedCallback);
    
        /// <summary>
        /// Checks if an interstitial ad is ready to be shown
        /// </summary>
        /// <returns>True if interstitial is ready, false otherwise</returns>
        bool IsInterstitialReady();
    
        /// <summary>
        /// Checks if a rewarded ad is ready to be shown
        /// </summary>
        /// <returns>True if rewarded ad is ready, false otherwise</returns>
        bool IsRewardedAdReady();
    }
}