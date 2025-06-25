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
            return $"{hours}h {minutes}m {seconds}s";
        }
        else if (minutes > 0)
        {
            return $"{minutes}m {seconds}s";
        }
        else
        {
            return $"{seconds}s";
        }
    }

    public void UpdateTimeDisplay(float timeInSeconds) => timeText.text = GetFormattedTime(timeInSeconds);
}