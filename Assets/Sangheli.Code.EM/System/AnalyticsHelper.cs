using Unity.Services.Analytics;

public static class AnalyticsHelper
{
    public static void Checkpoint()
    {
        var save = G.I.SaveProvider.Get();
        var customEvent =  new CustomEvent("checkpoint")
        {
            { "playtime", (int) save.timeSinceStart },
            { "level", save.level }
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
    
    public static void OnDsClicked()
    {
        var customEvent =  new CustomEvent("discord")
        {
            { "clicked",true },
        };

        AnalyticsService.Instance.RecordEvent(customEvent);
    }
}