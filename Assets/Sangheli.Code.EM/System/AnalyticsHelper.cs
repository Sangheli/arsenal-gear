using System;
using System.Collections.Generic;
using System.Text;
using Unity.Services.Analytics;
#if UNITY_EDITOR
using UnityEngine;
#endif

public static class AnalyticsHelper
{
    public static void Checkpoint(EndRoundFadeEnum fadeType)
    {
        const string eventName = "checkpoint";
        
        var save = G.I.SaveProvider.Get();
        var customEvent =  new CustomEvent(eventName)
        {
            { "playtime", (int) save.timeSinceStart },
            { "level", save.level },
            { "money", save.totalEnergy },
            { "loadcount", save.loadTimes },
            { "winState", fadeType.ToString() },
        };
        
        Send(customEvent, eventName);
    }
    
    public static void OnYoutubeClicked()
    {
        const string eventName = "youtube";
        var customEvent =  new CustomEvent(eventName)
        {
            { "clicked",true },
        };

        Send(customEvent, eventName);
    }
    
    public static void OnDsClicked()
    {
        const string eventName = "discord";
        var customEvent =  new CustomEvent(eventName)
        {
            { "clicked",true },
        };

        Send(customEvent, eventName);
    }
    
    public static void OnFirstGameLaunch()
    {
        const string eventName = "ce_newGame";
        var customEvent = new CustomEvent(eventName);
        Send(customEvent,eventName);
    }
    
    public static void OnShopEntered()
    { 
        const string eventName = "ce_shopEntered";
        
        var save = G.I.SaveProvider.Get();
        var customEvent =  new CustomEvent(eventName)
        {
            { "playtime", (int) save.timeSinceStart },
            { "level", save.level },
            { "money", save.totalEnergy },
            { "loadcount", save.loadTimes },
        };
        
        Send(customEvent, eventName);
    }
    
    public static void OnInterstitialAdRequested()
    {
        const string eventName = "ce_adIntRequested";
        var customEvent = new CustomEvent(eventName);
        Send(customEvent, eventName);
    }
    
    public static void OnInterstitialAdShown()
    {
        const string eventName = "ce_adIntShown";
        var customEvent = new CustomEvent(eventName);
        Send(customEvent,eventName);
    }
    
    public static void OnInterstitialAdFinished()
    {
        const string eventName = "ce_adIntFinished";
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
            { "rewarded", rewarded }
        };
        
        Send(customEvent, eventName);
    }
    
    private static void Send(CustomEvent customEvent, string name)
    {
#if !UNITY_EDITOR
        AnalyticsService.Instance.RecordEvent(customEvent);
#else
        
        var sb = new StringBuilder();
        sb.Append($"[Event][{name}] ");
        
        foreach (KeyValuePair<string, object> parameter in customEvent)
            sb.Append($"{parameter.Key}: [{parameter.Value}] ");
        
        Debug.Log(sb.ToString());
#endif
    }
}