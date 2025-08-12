using System;
using UnityEngine;

namespace SangheliCore.Adv
{
    /// <summary>
    /// Mock implementation of IAdvertisement for testing purposes
    /// </summary>
    public class MockAdvertisement : IAdvertisement
    {
        private bool _interstitialReady = true;
        private bool _rewardedAdReady = true;
        
        /// <summary>
        /// Event triggered when an interstitial ad finishes showing
        /// </summary>
        public event Action OnInterstitialFinished;
        
        /// <summary>
        /// Event triggered when a rewarded ad finishes showing
        /// </summary>
        public event Action<bool> OnRewardedAdFinished;
        
        /// <summary>
        /// Shows an interstitial ad (mock implementation)
        /// </summary>
        public void ShowInterstitial()
        {
            // Send analytics event for interstitial ad request
            global::AnalyticsHelper.OnInterstitialAdRequested();
            
            if (!IsInterstitialReady()) return;
            
            Debug.Log("Showing mock interstitial ad");
            
            // Send analytics event for interstitial ad shown
            global::AnalyticsHelper.OnInterstitialAdShown();
            
            // Simulate ad display with a slight delay
            // In a real implementation, this would be handled by the ad network
            _interstitialReady = false;
            
            // Simulate ad completion after a short time
            // In a real implementation, this would be called by the ad network callbacks
            OnInterstitialFinished?.Invoke();
            
            // Send analytics event for interstitial ad finished
            global::AnalyticsHelper.OnInterstitialAdFinished();
            
            // Make ad available again after showing
            _interstitialReady = true;
        }
        
        /// <summary>
        /// Shows a rewarded ad (mock implementation)
        /// </summary>
        /// <param name="rewardedCallback">Callback with true if reward should be given, false otherwise</param>
        public void ShowRewardedAd(Action<bool> rewardedCallback)
        {
            // Send analytics event for rewarded ad request
            global::AnalyticsHelper.OnRewardedAdRequested();
            
            if (!IsRewardedAdReady()) return;
            
            Debug.Log("Showing mock rewarded ad");
            
            // Send analytics event for rewarded ad shown
            global::AnalyticsHelper.OnRewardedAdShown();
            
            // Simulate ad display with a slight delay
            // In a real implementation, this would be handled by the ad network
            _rewardedAdReady = false;
            
            // Simulate ad completion after a short time
            // In a real implementation, this would be called by the ad network callbacks
            bool shouldReward = true; // Mock always rewards
            OnRewardedAdFinished?.Invoke(shouldReward);
            rewardedCallback?.Invoke(shouldReward);
            
            // Send analytics event for rewarded ad finished with reward status
            global::AnalyticsHelper.OnRewardedAdFinished(shouldReward);
            
            // Make ad available again after showing
            _rewardedAdReady = true;
        }
        
        /// <summary>
        /// Checks if an interstitial ad is ready to be shown
        /// </summary>
        /// <returns>True if interstitial is ready, false otherwise</returns>
        public bool IsInterstitialReady()
        {
            return _interstitialReady;
        }
        
        /// <summary>
        /// Checks if a rewarded ad is ready to be shown
        /// </summary>
        /// <returns>True if rewarded ad is ready, false otherwise</returns>
        public bool IsRewardedAdReady()
        {
            return _rewardedAdReady;
        }
    }
}