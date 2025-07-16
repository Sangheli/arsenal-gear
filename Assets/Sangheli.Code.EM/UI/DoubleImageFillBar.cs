using UnityEngine;
using UnityEngine.UI;

public class DoubleImageFillBar : MonoBehaviour
{
    public Text text;
    public Image fillImage;
    public Image fillImageBack;

    public void Fill(float value, float fullValue)
    {
        var fill = value / fullValue;
        fillImage.fillAmount = fill;
        fillImageBack.fillAmount = 1-fill;
        text.text = $"{value:F1}/{fullValue:F1}";
    }
}