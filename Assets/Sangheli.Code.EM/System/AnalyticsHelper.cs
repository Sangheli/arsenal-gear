using System;
using System.Collections.Generic;
using System.Text;
using Unity.Services.Analytics;
using UnityEngine;

public static class AnalyticsHelper
{
    public static void Checkpoint(EndRoundFadeEnum fadeType)
    {
        const string eventName = "ce_checkpoint";
        
        var save = G.I.SaveProvider.Get();
        var customEvent =  new CustomEvent(eventName)
        {
            { "c_playtime", (int) save.timeSinceStart },
            { "c_level", save.level },
            { "c_money", save.totalEnergy },
            { "c_loadcount", save.loadTimes },
            { "c_checkpointWinState", fadeType.ToString() },
            { "c_winCount", save.wins },
            { "c_loseCount", save.losses },
            { "c_shopVisits", save.shopVisits },
            { "c_restartsFromEndRound", G.I.RestartsFromEndRound },
        };
        
        Send(customEvent, eventName);
    }
    
    public static void OnYoutubeClicked()
    {
        const string eventName = "ce_youtube";
        var customEvent = new CustomEvent(eventName);
        Send(customEvent, eventName);
    }
    
    public static void OnDsClicked()
    {
        const string eventName = "ce_discord";
        var customEvent = new CustomEvent(eventName);
        Send(customEvent, eventName);
    }
    
    public static void OnFirstGameLaunch()
    {
        const string eventName = "ce_newGame";
        var customEvent = new CustomEvent(eventName);
        Send(customEvent,eventName);
    }
    
    public static void OnContinue()
    {
        const string eventName = "ce_continueGame";
        var customEvent = new CustomEvent(eventName);
        Send(customEvent,eventName);
    }
    
    public static void OnShopEntered()
    { 
        const string eventName = "ce_shopEntered";
        
        var save = G.I.SaveProvider.Get();
        var customEvent =  new CustomEvent(eventName)
        {
            { "c_playtime", (int) save.timeSinceStart },
            { "c_level", save.level },
            { "c_money", save.totalEnergy },
            { "c_loadcount", save.loadTimes },
            { "c_shopVisits", save.shopVisits },
        };
        
        Send(customEvent, eventName);
    }
    
    public static void OnInterstitialAdRequested()
    {
        const string eventName = "ce_adInterstitialRequested";
        var customEvent = new CustomEvent(eventName);
        Send(customEvent, eventName);
    }
    
    public static void OnInterstitialAdShown()
    {
        const string eventName = "ce_adInterstitialShown";
        var customEvent = new CustomEvent(eventName);
        Send(customEvent,eventName);
    }
    
    public static void OnInterstitialAdFinished()
    {
        const string eventName = "ce_adInterstitialFinished";
        var customEvent = new CustomEvent(eventName);
        Send(customEvent,eventName);
    }
    
    public static void OnRewardedAdRequested()
    {
        const string eventName = "ce_adRewardedRequested";
        var customEvent = new CustomEvent(eventName);
        Send(customEvent,eventName);
    }
    
    public static void OnRewardedAdShown()
    {
        const string eventName = "ce_adRewardedShown";
        var customEvent = new CustomEvent(eventName);
        Send(customEvent,eventName);
    }
    
    public static void OnRewardedAdFinished(bool rewarded)
    {
        const string eventName = "ce_adRewardedFinished";
        var customEvent = new CustomEvent(eventName)
        {
            { "c_rewardedShown", rewarded }
        };
        
        Send(customEvent, eventName);
    }
    
    public static void OnStartRound()
    {
        var save = G.I.SaveProvider.Get();
        var bought =  save.paramContainer.GetTempBought();
        var enemyBought = save.enemyUpgradeContainer.GetTempBought();
        
        if(string.IsNullOrEmpty(bought) && string.IsNullOrEmpty(enemyBought))
            return;
        
        const string eventName = "ce_onStartRound";
        var customEvent = new CustomEvent(eventName);

        // Combine both upgrade information into a single parameter using StringBuilder
        var sb = new StringBuilder();
        if (!string.IsNullOrEmpty(bought))
            sb.Append(bought);
        if (!string.IsNullOrEmpty(enemyBought))
        {
            if (sb.Length > 0)
                sb.Append("\n");
            sb.Append(enemyBought);
        }
        customEvent.Add("c_level", save.level);
        customEvent.Add("c_upgradesBought", sb.ToString());

        Send(customEvent, eventName);
    }
    
    public static void OnLevelProgressMilestone(int milestone)
    {
        string eventName = $"ce_progress_{milestone:D3}";
        var customEvent = new CustomEvent(eventName);
        Send(customEvent, eventName);
    }
    
    public static void CheckLevelProgressMilestones(int previousLevel, int newLevel)
    {
        // Get the max level
        int maxLevel = G.I.maxLevel;
        if (maxLevel <= 0) return;
        
        // Define the milestones to track
        int[] milestones = { 1, 3, 5,7, 10,12, 15,17, 20, 25, 30, 35, 40, 45, 50, 
                           55, 60, 65, 70, 75, 80, 85, 90, 95, 100 };
        
        // Check each milestone
        foreach (int milestone in milestones)
        {
            // Calculate the level that represents this milestone
            int milestoneLevel = Mathf.CeilToInt(maxLevel * milestone / 100f);
            
            // Check if we crossed this milestone
            if (previousLevel < milestoneLevel && newLevel >= milestoneLevel)
            {
                OnLevelProgressMilestone(milestone);
            }
        }
    }

    private static void Send(CustomEvent customEvent, string name)
    {
#if !UNITY_EDITOR
        AnalyticsService.Instance.RecordEvent(customEvent);
#else
        
        var sb = new StringBuilder();
        sb.Append($"[Event][{name}]\n");
        
        foreach (KeyValuePair<string, object> parameter in customEvent)
            sb.Append($"{parameter.Key}: [{parameter.Value}]\n");
        
        Debug.Log(sb.ToString());
#endif
    }
}