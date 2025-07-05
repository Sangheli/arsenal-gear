using UnityEngine;

public static class MediaUrlOpener
{
    private const string YoutubeUrl = "https://www.youtube.com/@xboxseriousgamer";
    private const string DsUrl = "https://discord.gg/UKkDDCyV97";

    public static void OpenYoutube()
    {
        AnalyticsHelper.OnYoutubeClicked();
        OpenURL(YoutubeUrl);
    }

    public static void OpenDs()
    {
        AnalyticsHelper.OnDsClicked();
        OpenURL(DsUrl);
    }

    public static void OpenURL(string url)
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
    }
} 