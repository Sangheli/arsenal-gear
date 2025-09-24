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
    public Color winTextColor = Color.black;
    public Color failTextColor = Color.black;
    public Color demoTextColor = Color.black;
    public Color playerTerminatedTextColor = Color.black;

    private static readonly int FadeAmount = Shader.PropertyToID("_FadeAmount");
    private static readonly int ColorKey = Shader.PropertyToID("_Color");
    private Coroutine _fadeCoroutine;
    private Material _mat;

    public Color failColor;
    public Color blueColor;
    public Color demoColor;
    public Color playerTerminatedColor;

    public void OnLoadInit()
    {
        // Создаём копию материала, чтобы не менять sharedMaterial
        if (ViewImage.material != null)
            ViewImage.material = Instantiate(ViewImage.material);
        
        _mat = ViewImage.material;
        
        ResetView();
        gameObject.SetActive(false);
    }

    public IEnumerator DoFade(ILevelTargetEnd levelTargetEnd, EndRoundFadeEnum fadeType)
    {
        var targetColor = GetInfo(levelTargetEnd, fadeType, out var textColor, out var valueText);
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

    private Color GetInfo(ILevelTargetEnd levelTargetEnd, EndRoundFadeEnum fadeType, out Color textColor, out string valueText)
    {
        var save = G.I?.SaveProvider?.Get();
        Color targetColor;

        switch (fadeType)
        {
            case EndRoundFadeEnum.RoundFail:
                targetColor = failColor;
                textColor = failTextColor;
                valueText = G.I.localization.Get("fail_text");
                
                // Check if we should use win color instead of fail color
                
                bool useWinColor = false;
                if (save != null)
                {
                    var levelId = save.level;
                    var levelConfig = G.I.LevelDataConfig.Get(levelId, false);
                    if (levelConfig != null)
                        useWinColor = levelConfig.useWinColorOnFail;
                }
                
                // Override with win color if needed
                if (useWinColor)
                {
                    targetColor = blueColor;
                    textColor = winTextColor;
                }
                break;
            case EndRoundFadeEnum.RoundWin:
                targetColor = blueColor;
                textColor = winTextColor;
                valueText = G.I.localization.Get(levelTargetEnd.Localization);
                break;
            case EndRoundFadeEnum.EndGame:
                targetColor = demoColor;
                textColor = demoTextColor;
                valueText = G.I.localization.Get("demo_done");
                break;
            case EndRoundFadeEnum.RoundTerminated:
                targetColor = playerTerminatedColor;
                textColor = playerTerminatedTextColor;
                valueText = G.I.localization.Get("player_terminated_text");
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