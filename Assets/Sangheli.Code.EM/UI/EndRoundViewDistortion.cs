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

    private static readonly int FadeAmount = Shader.PropertyToID("_FadeAmount");
    private static readonly int ColorKey = Shader.PropertyToID("_Color");
    private Coroutine _fadeCoroutine;
    private Material _mat;

    public Color failColor;
    public Color blueColor;

    private void Awake()
    {
        // Создаём копию материала, чтобы не менять sharedMaterial
        if (ViewImage.material != null)
            ViewImage.material = Instantiate(ViewImage.material);
        
        _mat = ViewImage.material;
    }

    public IEnumerator DoFade(bool isFail)
    {
        _mat.SetColor(ColorKey, isFail ? failColor : blueColor);
        var startValue = MinValue;
        var endValue = MaxValue;
        var elapsed = 0f;
        
        var startColor = Color.clear;
        var endColor = isFail ? failTextColor : winTextColor;
        resultText.color = startColor;
        resultText.text = isFail ? failText : winText;

        while (elapsed < animTime)
        {
            elapsed += Time.deltaTime;
            var t = Mathf.Clamp01(elapsed / animTime);
            var currentValue = Mathf.Lerp(startValue, endValue, t);
            _mat.SetFloat(FadeAmount, currentValue);
            resultText.color = Color.Lerp(startColor, endColor, t);
            yield return null;
        }
        
        resultText.color = endColor;
        _mat.SetFloat(FadeAmount, endValue);
    }

    public void ResetView()
    {
        _mat.SetFloat(FadeAmount, MinValue);
        resultText.color = Color.clear;
    }
}