using UnityEngine;
using UnityEngine.UI;

public class GameTimeDisplay : MonoBehaviour
{
    public Text timeText;
    
    public static string GetFormattedTime(float timeSinceStart)
    {

        int hours = (int)(timeSinceStart / 3600);
        int minutes = (int)((timeSinceStart % 3600) / 60);
        int seconds = (int)(timeSinceStart % 60);

        // Формируем строку в зависимости от времени
        if (hours > 0)
        {
            return $"{hours}{G.I.localization.Get("h")} {minutes}{G.I.localization.Get("m")} {seconds}{G.I.localization.Get("s")}";
        }
        else if (minutes > 0)
        {
            return $"{minutes}{G.I.localization.Get("m")} {seconds}{G.I.localization.Get("s")}";
        }
        else
        {
            return $"{seconds}{G.I.localization.Get("s")}";
        }
    }

    public void UpdateTimeDisplay(float timeInSeconds) => timeText.text = GetFormattedTime(timeInSeconds);
}