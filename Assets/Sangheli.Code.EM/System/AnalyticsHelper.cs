using Unity.Services.Analytics;
#if UNITY_EDITOR
using UnityEngine;
#endif

public static class AnalyticsHelper
{
    public static void Checkpoint()
    {
        var save = G.I.SaveProvider.Get();
        var customEvent =  new CustomEvent("checkpoint")
        {
            { "playtime", (int) save.timeSinceStart },
            { "level", save.level },
            { "money", save.totalEnergy },
            { "loadcount", save.loadTimes },
        };

        AnalyticsService.Instance.RecordEvent(customEvent);
        
#if UNITY_EDITOR
        Debug.Log($"Analytics Event: checkpoint, Playtime: {(int) save.timeSinceStart}, Level: {save.level}, Money: {save.totalEnergy}, LoadCount: {save.loadTimes}");
#endif
    }
    
    public static void OnYoutubeClicked()
    {
        var customEvent =  new CustomEvent("youtube")
        {
            { "clicked",true },
        };

        AnalyticsService.Instance.RecordEvent(customEvent);
        
#if UNITY_EDITOR
        Debug.Log("Analytics Event: youtube clicked");
#endif
    }
    
    public static void OnDsClicked()
    {
        var customEvent =  new CustomEvent("discord")
        {
            { "clicked",true },
        };

        AnalyticsService.Instance.RecordEvent(customEvent);
        
#if UNITY_EDITOR
        Debug.Log("Analytics Event: discord clicked");
#endif
    }
}