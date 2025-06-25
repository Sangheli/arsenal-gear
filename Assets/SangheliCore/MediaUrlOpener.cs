using UnityEngine;

public static class MediaUrlOpener
{
    private const string YoutubeUrl = "https://www.youtube.com/@xboxseriousgamer";
    private const string DsUrl = "https://discord.gg/UKkDDCyV97";

    public static void OpenYoutube() => OpenUrl(YoutubeUrl);
    public static void OpenDs() => OpenUrl(DsUrl);

    private static void OpenUrl(string url)
    {
        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
#pragma warning disable CS0618
            Application.ExternalEval($"window.open(\"{url}\",\"_blank\")");
#pragma warning restore CS0618
        }
        else
        {
            Application.OpenURL(url);
        }
        
        AnalyticsHelper.OnYoutubeClicked();
    }
} 