using System;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class DistortionUI : MonoBehaviour
{
    public Image distDown;
    public Image distUp;
    public float MaxValue;
    public float MinValue;
    public float animTime = 0.1f;
    public Text[] texts;
    
    private static readonly int FadeAmount = Shader.PropertyToID("_FadeAmount");
    private Coroutine _fadeCoroutine;
    private Material _matDown;
    private Material _matUp;

    private Color[] _startTextColors;
    public float starGameWaitTime = 1;
    public float endRoundWaitTime = 1;
    
    public CanvasGroup canvasGroup;

    private void OnValidate()
    {
        canvasGroup ??= GetComponent<CanvasGroup>();
    }

    private void Awake()
    {
        _startTextColors = new Color[texts.Length];
        for (int i = 0; i < texts.Length; i++)
            _startTextColors[i] = texts[i].color;
        
        _matDown = GetMat(distDown);
        _matUp = GetMat(distUp);
        DoBlock(false);
        
        StartCoroutine(StartFadeOut());
    }

    private Material GetMat(Image img)
    {
        // Создаём копию материала, чтобы не менять sharedMaterial
        if (img.material != null)
            img.material = Instantiate(img.material);

        return img.material;
    }

    private IEnumerator StartFadeOut()
    {
        DoBlock(true);
        _matDown.SetFloat(FadeAmount, MaxValue);
        _matUp.SetFloat(FadeAmount, MaxValue);
        for (int i = 0; i < texts.Length; i++)
            texts[i].color = _startTextColors[i];
        yield return new WaitForSeconds(starGameWaitTime);
        yield return DoFade(forward: false);
        DoBlock(false);
    }

    public IEnumerator DoFade(bool forward)
    {
        distDown.gameObject.SetActive(forward);
        distUp.gameObject.SetActive(!forward);

        var startValue = forward ? MinValue : MaxValue;
        var endValue = forward ? MaxValue : MinValue;
        var endValue1 = forward ? 1f : 0f;
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
            
            for (int i = 0; i < texts.Length; i++)
                texts[i].color = Color.Lerp(startColors[i], endColors[i], t);
            
            if (forward)
            {
                _matDown.SetFloat(FadeAmount, currentValue);
            }
            else
            {
                _matUp.SetFloat(FadeAmount, currentValue);
            }
            
            yield return null;
        }
        
        for (int i = 0; i < texts.Length; i++)
            texts[i].color = endColors[i];
        
        if (forward)
        {
            _matDown.SetFloat(FadeAmount, endValue);    
        }
        else
        {
            _matUp.SetFloat(FadeAmount, endValue);
        }
        
    }

    public void DoBlock(bool state)
    {
        distDown.raycastTarget = state;
        distUp.raycastTarget = state;
    }
}