using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

public class EnemyRender : MonoBehaviour
{
    private readonly int _clipUvUp = Shader.PropertyToID("_ClipUvUp");
    private readonly int _hitEffectBlend = Shader.PropertyToID("_HitEffectBlend");
    public readonly int ColorProperty = Shader.PropertyToID("_Color");

    public string borderColorProp = "_Color";
    private int _borderColor = 0;
    
    
    [Space]
    public Renderer viewRenderer;
    [FormerlySerializedAs("ramkaRender")] public Renderer borderRender;
    public Renderer[] borderRenderArr;
    public Renderer backRender;
    public Renderer shadowRender;

    public MaterialPropertyBlock PropertyBlock;
    public MaterialPropertyBlock PropertyBlockBack;
    public MaterialPropertyBlock PropertyBlockRamka;

    public Color OriginalColor { get; internal set; }
    public Color OriginalRamkaColor { get; private set; }
    
    private Vector3 _originalScale;
    private Vector3 _originalScaleShadow;

    private Coroutine _bounceRoutine;
    private Coroutine _bounceShadowRoutine;
    private Coroutine _flashRoutine;
    private Coroutine _critColorRoutine;
    
    public float punchScale = 1.2f;
    public float punchScaleCrit = 1.2f*1.2f;
    public float scale = 1;
    public float duration = 0.2f;
    public float maxFlashValue = 0.2f;
    public float deathAnimDuration = 0.3f;
    
    private void Awake()
    {
        _borderColor = Shader.PropertyToID(borderColorProp);
        
        _originalScale = viewRenderer.transform.localScale;
        _originalScaleShadow = shadowRender.transform.localScale;

        PropertyBlock = new MaterialPropertyBlock();
        viewRenderer.GetPropertyBlock(PropertyBlock);
        
        PropertyBlockRamka = new MaterialPropertyBlock();
        borderRender.GetPropertyBlock(PropertyBlockRamka);
        
        PropertyBlockBack = new MaterialPropertyBlock();
        backRender.GetPropertyBlock(PropertyBlockBack);
        
        OriginalColor = viewRenderer.material.color;
        
    }
    
    public void ApplyPropertyBack(Color bossColor)
    {
        PropertyBlockBack.SetColor(ColorProperty, bossColor);
        ApplyPropertyBack();
    }

    public virtual void ApplyPropertyBack()
    {
        backRender.SetPropertyBlock(PropertyBlockBack);
    }
    
    public void ApplyPropertyView(Color backColor)
    {
        PropertyBlock.SetColor(ColorProperty, backColor);
        ApplyPropertyView();
    }

    public virtual void ApplyPropertyView()
    {
        viewRenderer.SetPropertyBlock(PropertyBlock);
    }

    public void SetScale(float scale)
    {
        this.scale = scale;
        viewRenderer.transform.localScale = _originalScale*scale;
        shadowRender.transform.localScale = _originalScaleShadow*scale;
    }

    public void SetFillValue(float targetClipValue)
    {
        PropertyBlock.SetFloat(_clipUvUp, targetClipValue);
        ApplyPropertyView();
    }

    public void SetHitValue(float value)
    {
        PropertyBlock.SetFloat(_hitEffectBlend, value);
        ApplyPropertyView();
    }

    public void DoBounceAnimation(bool isCrit)
    {
        // Остановка предыдущих корутин
        if (_bounceRoutine != null)
            StopCoroutine(_bounceRoutine);
        if (_bounceShadowRoutine != null)
            StopCoroutine(_bounceShadowRoutine);

        var tempScale = isCrit ? punchScaleCrit : punchScale;
        var addonScale = tempScale - 1;
        addonScale /= scale;
        
        // Запуск и кеширование новых корутин
        _bounceRoutine = StartCoroutine(GameAnimation.Bounce(viewRenderer.transform, _originalScale*scale, 1+addonScale, duration));
        _bounceShadowRoutine = StartCoroutine(GameAnimation.Bounce(shadowRender.transform, _originalScaleShadow*scale, 1+addonScale, duration));
    }

    public void FlashEffect(bool isCrit)
    {
        if (_flashRoutine != null)
            StopCoroutine(_flashRoutine);
        
        _flashRoutine = StartCoroutine(GameAnimation.FlashEffect(SetHitValue, isCrit, maxFlashValue, duration));
    }

    public void ResetItem()
    {
        if (_flashRoutine != null)
            StopCoroutine(_flashRoutine);
        if (_bounceRoutine != null)
            StopCoroutine(_bounceRoutine);
        if (_bounceShadowRoutine != null)
            StopCoroutine(_bounceShadowRoutine);
        if (_critColorRoutine != null)
            StopCoroutine(_critColorRoutine);
        
        var transform2 = viewRenderer.transform;
        var transform3 = shadowRender.transform;
        transform2.localScale = _originalScale;
        transform3.localScale = _originalScaleShadow;
        
        // Сброс вращения
        transform2.localRotation = Quaternion.identity;
        transform3.localRotation = Quaternion.identity;
    }

    public void CritColorEffect()
    {
        if (_critColorRoutine != null) StopCoroutine(_critColorRoutine);
        _critColorRoutine = StartCoroutine(CritColorCoroutine());
    }
    
    public IEnumerator CritColorCoroutine()
    {
        PropertyBlock.SetColor(ColorProperty, Color.red);
        ApplyPropertyView();
        yield return new WaitForSeconds(duration);
        PropertyBlock.SetColor(ColorProperty, OriginalColor);
        ApplyPropertyView();
    }

    public void SetSortingOrder(SortingOrderData data)
    {
        borderRender.sortingOrder = data.ramka;
        foreach (var x in borderRenderArr) x.sortingOrder = data.ramka;
        viewRenderer.sortingOrder = data.view;
        shadowRender.sortingOrder = data.shadow;
        backRender.sortingOrder = data.back;
    }

    public IEnumerator DeathAnimation()
    {
        var transform1 = viewRenderer.transform;
        var transform2 = shadowRender.transform;
        
        StartCoroutine(GameAnimation.ShrinkAndDisappear(transform1, transform1.localScale, deathAnimDuration));
        yield return StartCoroutine(GameAnimation.ShrinkAndDisappear(transform2, transform2.localScale, deathAnimDuration));
    }

    public void SetFullColor(Color color, Color borderColor)
    {
        OriginalColor = color;
        OriginalRamkaColor = borderColor;

        var colorBack = color;
        colorBack.a = 100f / 255f;
        
        ApplyPropertyBack(colorBack);
        ApplyPropertyView(OriginalColor);
        ApplyBorderColor();
    }
    
    public void SetBorderColor(Color borderColor)
    {
        OriginalRamkaColor = borderColor;
        ApplyBorderColor();
    }

    public void ApplyBorderColor()
    {
        PropertyBlockRamka.SetColor(_borderColor, OriginalRamkaColor);
        borderRender.SetPropertyBlock(PropertyBlockRamka);
        foreach (var x in borderRenderArr) x.SetPropertyBlock(PropertyBlockRamka);
    }

    public void ApplyBorderColor(Color black)
    {
        OriginalRamkaColor = black;
        ApplyBorderColor();
    }
}