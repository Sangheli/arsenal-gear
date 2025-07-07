using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class DistortionUI : MonoBehaviour
{
    public Image ViewImage;
    public float MaxValue;
    public float MinValue;
    public float animTime = 0.1f;
    public Text[] texts;
    
    private static readonly int FadeAmount = Shader.PropertyToID("_FadeAmount");
    private Coroutine _fadeCoroutine;
    private Material _mat;

    private Color[] _startTextColors;
    public float starGameWaitTime = 1;
    public float endRoundWaitTime = 1;
    
    private void Awake()
    {
        _startTextColors = new Color[texts.Length];
        for (int i = 0; i < texts.Length; i++)
            _startTextColors[i] = texts[i].color;
        
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
        for (int i = 0; i < texts.Length; i++)
            texts[i].color = _startTextColors[i];
        yield return new WaitForSeconds(starGameWaitTime);
        yield return DoFade(forward: false);
        DoBlock(false);
    }

    public IEnumerator DoFade(bool forward)
    {
        var startValue = forward ? MinValue : MaxValue;
        var endValue = forward ? MaxValue : MinValue;
        var elapsed = 0f;

        Color[] startColors = new Color[texts.Length];
        Color[] endColors = new Color[texts.Length];
        for (int i = 0; i < texts.Length; i++)
        {
            startColors[i] = forward ? Color.clear : _startTextColors[i];
            endColors[i] = forward ? _startTextColors[i] : Color.clear;
            texts[i].color = startColors[i];
        }

        if (forward) G.I.Container.pooledAudioPlayer.PlayWithRandomPitch(G.I.Data.soundContainer.loadinScreen);

        while (elapsed < animTime)
        {
            elapsed += Time.deltaTime;
            var t = Mathf.Clamp01(elapsed / animTime);
            var currentValue = Mathf.Lerp(startValue, endValue, t);

            _mat.SetFloat(FadeAmount, currentValue);
            for (int i = 0; i < texts.Length; i++)
                texts[i].color = Color.Lerp(startColors[i], endColors[i], t);

            yield return null;
        }
        
        for (int i = 0; i < texts.Length; i++)
            texts[i].color = endColors[i];
        _mat.SetFloat(FadeAmount, endValue);
    }

    public void DoBlock(bool state)
    {
        ViewImage.raycastTarget = state;
    }
}