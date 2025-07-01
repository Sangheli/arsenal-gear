using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class EndRoundViewDistortion : MonoBehaviour
{
    public Image ViewImage;
    public float MaxValue;
    public float MinValue;
    public float animTime = 0.1f;
    
    public Text resultText;
    public string winText = "Boss Defeated";
    public string failText = "HP out";
    public Color winTextColor = Color.black;
    public Color failTextColor = Color.black;
    public Color demoTextColor = Color.black;

    private static readonly int FadeAmount = Shader.PropertyToID("_FadeAmount");
    private static readonly int ColorKey = Shader.PropertyToID("_Color");
    private Coroutine _fadeCoroutine;
    private Material _mat;

    public Color failColor;
    public Color blueColor;
    public Color demoColor;

    private void Awake()
    {
        // Создаём копию материала, чтобы не менять sharedMaterial
        if (ViewImage.material != null)
            ViewImage.material = Instantiate(ViewImage.material);
        
        _mat = ViewImage.material;
    }

    public void OnLoadInit()
    {
        ResetView();
        gameObject.SetActive(false);
    }

    public IEnumerator DoFade(EndRoundFadeEnum fadeType)
    {
        var targetColor = GetInfo(fadeType, out var textColor, out var valueText);
        _mat.SetColor(ColorKey, targetColor);
        var startValue = MinValue;
        var endValue = MaxValue;
        var elapsed = 0f;
        
        var startColor = Color.clear;
        resultText.color = startColor;
        resultText.text = valueText.ReplaceEscapedNewlines();

        while (elapsed < animTime)
        {
            elapsed += Time.deltaTime;
            var t = Mathf.Clamp01(elapsed / animTime);
            var currentValue = Mathf.Lerp(startValue, endValue, t);
            _mat.SetFloat(FadeAmount, currentValue);
            resultText.color = Color.Lerp(startColor, textColor, t);
            yield return null;
        }
        
        resultText.color = textColor;
        _mat.SetFloat(FadeAmount, endValue);
    }

    private Color GetInfo(EndRoundFadeEnum fadeType, out Color textColor, out string valueText)
    {
        Color targetColor;
        switch (fadeType)
        {
            case EndRoundFadeEnum.Fail:
                targetColor = failColor;
                textColor = failTextColor;
                valueText = failText;
                break;
            case EndRoundFadeEnum.Win:
                targetColor = blueColor;
                textColor = winTextColor;
                valueText = winText;
                break;
            case EndRoundFadeEnum.EndDemo:
                targetColor = demoColor;
                textColor = demoTextColor;
                valueText = "Demo complete\nThanks for playing\nReload page to start a new game";
                break;
            default:
                targetColor = Color.clear;
                textColor = Color.clear;
                valueText = string.Empty;
                break;
        }

        return targetColor;
    }

    public void ResetView()
    {
        _mat.SetFloat(FadeAmount, MinValue);
        resultText.color = Color.clear;
    }
}