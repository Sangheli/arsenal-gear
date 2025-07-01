using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class DistortionUI : MonoBehaviour
{
    public Image ViewImage;
    public float MaxValue;
    public float MinValue;
    public float animTime = 0.1f;
    public Text text;
    
    private static readonly int FadeAmount = Shader.PropertyToID("_FadeAmount");
    private Coroutine _fadeCoroutine;
    private Material _mat;

    private Color _startTextColor;
    public float starGameWaitTime = 1;
    public float endRoundWaitTime = 1;
    
    private void Awake()
    {
        _startTextColor = text.color;
        
        // Создаём копию материала, чтобы не менять sharedMaterial
        if (ViewImage.material != null)
            ViewImage.material = Instantiate(ViewImage.material);
        
        _mat = ViewImage.material;
        DoBlock(false);
        
        StartCoroutine(StartFadeOut());
    }

    private IEnumerator StartFadeOut()
    {
        DoBlock(true);
        _mat.SetFloat(FadeAmount, MaxValue);
        text.color = _startTextColor;
        yield return new WaitForSeconds(starGameWaitTime);
        yield return DoFade(forward: false);
        DoBlock(false);
    }

    public IEnumerator DoFade(bool forward)
    {
        var startValue = forward ? MinValue : MaxValue;
        var endValue = forward ? MaxValue : MinValue;
        var elapsed = 0f;

        var startColor = forward ? Color.clear : _startTextColor;
        var endColor = forward ? _startTextColor : Color.clear;

        text.color = startColor;

        if (forward) G.I.Container.pooledAudioPlayer.PlayWithRandomPitch(G.I.Data.soundContainer.loadinScreen);

        while (elapsed < animTime)
        {
            elapsed += Time.deltaTime;
            var t = Mathf.Clamp01(elapsed / animTime);
            var currentValue = Mathf.Lerp(startValue, endValue, t);

            _mat.SetFloat(FadeAmount, currentValue);
            text.color = Color.Lerp(startColor, endColor, t);

            yield return null;
        }
        
        text.color = endColor;
        _mat.SetFloat(FadeAmount, endValue);
    }

    public void DoBlock(bool state)
    {
        ViewImage.raycastTarget = state;
    }
}