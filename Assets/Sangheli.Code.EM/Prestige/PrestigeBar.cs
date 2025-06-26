using UnityEngine;
using UnityEngine.UI;

public class PrestigeBar : MonoBehaviour
{
    public Text text;
    public Image fillImage;
    public Image fillImageBack;

    public void Fill(float time, float fullTime)
    {
        var fill = time / fullTime;
        fillImage.fillAmount = fill;
        fillImageBack.fillAmount = 1-fill;
        text.text = $"{time:F2}/{fullTime:F2}";
    }
}