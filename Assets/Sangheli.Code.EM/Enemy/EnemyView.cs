using System.Collections;
using UnityEngine;

public class EnemyView : MonoBehaviour
{
    public Transform view;
    public Transform viewShadow;
    
    [Space]
    public Renderer ViewRenderer;
    public Renderer RamkaRender;
    public Renderer ShadowRender;
    
    [Space]
    public DeathVfx deathVfx;
    
    [Space]
    public bool manipulateRamkaColor;

    [Space]
    public float punchScale = 1.2f;
    public float punchScaleCrit = 1.2f*1.2f;
    public float duration = 0.2f;
    public float maxFlashValue = 0.2f;
    public float deathAnimDuration = 0.3f;
    
    protected Vector3 _originalScale;
    protected Vector3 _originalScaleShadow;
    protected Vector3 _originalScaleVfx;
    protected MaterialPropertyBlock _propertyBlock;
    private MaterialPropertyBlock _propertyBlockRamka;
    private Color _originalColor;
    private Color _originalRamkaColor;
    private Coroutine _flashRoutine;
    private Coroutine _critColorRoutine;
    private static readonly int ClipUvUp = Shader.PropertyToID("_ClipUvUp");
    private static readonly int HitEffectBlend = Shader.PropertyToID("_HitEffectBlend");
    protected static readonly int ColorProperty = Shader.PropertyToID("_Color");
    
    protected virtual void ApplyPropertyBlock()
    {
        ViewRenderer.SetPropertyBlock(_propertyBlock);
    }

    private void Awake()
    {
        _originalScale = view.transform.localScale;
        _originalScaleShadow = viewShadow.transform.localScale;
        _originalScaleShadow = deathVfx.transform.localScale;
        _propertyBlock = new MaterialPropertyBlock();
        ViewRenderer.GetPropertyBlock(_propertyBlock);
        
        _propertyBlockRamka = new MaterialPropertyBlock();
        RamkaRender.GetPropertyBlock(_propertyBlockRamka);
        
        _originalColor = ViewRenderer.material.color;
        
        if(manipulateRamkaColor)
            _originalRamkaColor = RamkaRender.material.color;
    }

    public void SetHitValue(float value)
    {
        _propertyBlock.SetFloat(HitEffectBlend, value);
        ApplyPropertyBlock();
    }

    public void DoBounceAnimation(bool isCrit = false)
    {
        float scale = isCrit ? punchScaleCrit : punchScale;
        StopCoroutine(nameof(GameAnimation.Bounce));
        StartCoroutine(GameAnimation.Bounce(view.transform ,_originalScale,scale,duration));
        StartCoroutine(GameAnimation.Bounce(viewShadow.transform ,_originalScaleShadow,scale,duration));
    }
    
    public void FlashEffect(bool isCrit = false)
    {
        if (_flashRoutine != null)
            StopCoroutine(_flashRoutine);
        
        _flashRoutine = StartCoroutine(GameAnimation.FlashEffect(SetHitValue, isCrit, maxFlashValue, duration));
    }

    private IEnumerator CritColorCoroutine()
    {
        _propertyBlock.SetColor(ColorProperty, Color.red);
        ApplyPropertyBlock();
        yield return new WaitForSeconds(duration);
        _propertyBlock.SetColor(ColorProperty, _originalColor);
        ApplyPropertyBlock();
    }

    public void ResetItem()
    {
        var transform2 = view.transform;
        var transform3 = viewShadow.transform;
        transform2.localScale = _originalScale;
        transform3.localScale = _originalScaleShadow;
        
        // Сброс вращения
        transform2.localRotation = Quaternion.identity;
        transform3.localRotation = Quaternion.identity;

        SetHitValue(0);
        SetFillValue(0);
        
        _propertyBlock.SetColor(ColorProperty, _originalColor);
        ApplyPropertyBlock();

        if (manipulateRamkaColor)
        {
            _propertyBlockRamka.SetColor(ColorProperty, _originalRamkaColor);
            RamkaRender.SetPropertyBlock(_propertyBlockRamka);
        }
    }

    public void SetFillValue(float targetClipValue)
    {
        _propertyBlock.SetFloat(ClipUvUp, targetClipValue);
        ApplyPropertyBlock();
    }

    public void SetSortingOrder(SortingOrderData data)
    {
        RamkaRender.sortingOrder = data.ramka;
        ViewRenderer.sortingOrder = data.view;
        ShadowRender.sortingOrder = data.shadow;
        deathVfx.SetOrder(data.ramka);
    }

    public void CritColorEffect()
    {
        if (_critColorRoutine != null) StopCoroutine(_critColorRoutine);
        _critColorRoutine = StartCoroutine(CritColorCoroutine());
    }
    
    public void PlayDeathVfx() => deathVfx.Play();
}