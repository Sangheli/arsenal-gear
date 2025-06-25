using Unity.Services.Analytics;

public static class AnalyticsHelper
{
    public static void Checkpoint()
    {
        var save = G.I.SaveProvider.Get();
        var customEvent =  new CustomEvent("checkpoint")
        {
            { "playtime", (int) save.timeSinceStart },
            { "prestige", save.prestige }
        };

        AnalyticsService.Instance.RecordEvent(customEvent);
    }
    
    public static void OnYoutubeClicked()
    {
        var customEvent =  new CustomEvent("youtube")
        {
            { "clicked",true },
        };

        AnalyticsService.Instance.RecordEvent(customEvent);
    }
}