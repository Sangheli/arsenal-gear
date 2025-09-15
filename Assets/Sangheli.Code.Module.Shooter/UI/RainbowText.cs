using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class RainbowText : MonoBehaviour
{
    public Color[] colors;

    public Text textComponent;
    private string originalText;

    private void OnValidate()
    {
        textComponent ??= GetComponent<Text>();
    }

    void Awake()
    {
        originalText = textComponent.text;
        ApplyRainbow();
    }

    void ApplyRainbow()
    {
        if (colors == null || colors.Length == 0 || string.IsNullOrEmpty(originalText))
            return;

        char[] chars = originalText.ToCharArray();
        int len = chars.Length;
        var sb = new System.Text.StringBuilder(len * 20); // приблизительная оценка

        for (int i = 0; i < len; i++)
        {
            int colorIndex = i;
            if (i >= len / 2)
                colorIndex = len - 1 - i;
            colorIndex = Mathf.Clamp(colorIndex, 0, colors.Length - 1);

            Color c = colors[colorIndex];
            string hex = ColorUtility.ToHtmlStringRGB(c);
            sb.Append($"<color=#{hex}>{chars[i]}</color>");
        }

        textComponent.supportRichText = true;
        textComponent.text = sb.ToString();
    }
} 