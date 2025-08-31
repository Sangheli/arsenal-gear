using System.Collections;
using UnityEngine;

public class EnemyRender : MonoBehaviour
{
    private readonly int _clipUvUp = Shader.PropertyToID("_ClipUvUp");
    private readonly int _hitEffectBlend = Shader.PropertyToID("_HitEffectBlend");
    private readonly int _colorProperty = Shader.PropertyToID("_Color");
    public string borderColorPropName = "_Color";
    public int borderColorProp = 0;
    
    [Space]
    public SpriteRenderer[] viewRenderArr;
    public SpriteRenderer[] borderRenderArr;
    public SpriteRenderer[] backRenderArr;
    public SpriteRenderer[] shadowRenderArr;

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
    
    public bool blockSpriteReplace { get; set; }
    
    private void Awake()
    {
        PropertyBlock = new MaterialPropertyBlock();
        viewRenderArr[0].GetPropertyBlock(PropertyBlock);
        
        PropertyBlockRamka = new MaterialPropertyBlock();
        borderRenderArr[0].GetPropertyBlock(PropertyBlockRamka);
        
        PropertyBlockBack = new MaterialPropertyBlock();
        backRenderArr[0].GetPropertyBlock(PropertyBlockBack);
        
        borderColorProp = Shader.PropertyToID(borderColorPropName);
        OriginalColor = viewRenderArr[0].material.color;

        foreach (var x in backRenderArr) x.gameObject.SetActive(false);
        foreach (var x in shadowRenderArr) x.gameObject.SetActive(false);
    }

    public void SetOriginalScale(Vector3 viewScale, bool overrideSet)
    {
        if(blockSpriteReplace && !overrideSet)
            return;
        
        _originalScale = viewScale;
        _originalScaleShadow = viewScale;
        viewRenderArr[0].transform.localScale = _originalScale*scale;
        shadowRenderArr[0].transform.localScale = _originalScaleShadow*scale;
    }
    
    public void ApplyPropertyBack(Color bossColor)
    {
        PropertyBlockBack.SetColor(_colorProperty, bossColor);
        ApplyPropertyBack();
    }

    public virtual void ApplyPropertyBack()
    {
        foreach (var x in backRenderArr) x.SetPropertyBlock(PropertyBlockBack);
    }
    
    public void ApplyPropertyView(Color backColor)
    {
        return;
        PropertyBlock.SetColor(_colorProperty, backColor);
        ApplyPropertyView();
    }

    public virtual void ApplyPropertyView()
    {
        foreach (var x in viewRenderArr) x.SetPropertyBlock(PropertyBlock);
    }

    public void SetScale(float scale)
    {
        this.scale = scale;
        viewRenderArr[0].transform.localScale = _originalScale*scale;
        shadowRenderArr[0].transform.localScale = _originalScaleShadow*scale;
    }

    public void SetFillValue(float targetClipValue)
    {
        return;
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
        _bounceRoutine = StartCoroutine(GameAnimation.Bounce(viewRenderArr[0].transform, _originalScale*scale, 1+addonScale, duration));
        _bounceShadowRoutine = StartCoroutine(GameAnimation.Bounce(shadowRenderArr[0].transform, _originalScaleShadow*scale, 1+addonScale, duration));
    }

    public void FlashEffect(bool isCrit)
    {
        if (_flashRoutine != null)
            StopCoroutine(_flashRoutine);
        
        _flashRoutine = StartCoroutine(GameAnimation.FlashEffect(SetHitValue, isCrit, maxFlashValue, duration));
    }

    public void ResetCoroutines()
    {
        if (_flashRoutine != null)
            StopCoroutine(_flashRoutine);
        if (_bounceRoutine != null)
            StopCoroutine(_bounceRoutine);
        if (_bounceShadowRoutine != null)
            StopCoroutine(_bounceShadowRoutine);
        if (_critColorRoutine != null)
            StopCoroutine(_critColorRoutine);
        
        var transform2 = viewRenderArr[0].transform;
        var transform3 = shadowRenderArr[0].transform;
        transform2.localScale = _originalScale;
        transform3.localScale = _originalScaleShadow;
        
        // Сброс вращения
        transform2.localRotation = Quaternion.identity;
        transform3.localRotation = Quaternion.identity;
    }
    
    public void ResetItem()
    {
        var transform2 = viewRenderArr[0].transform;
        var transform3 = shadowRenderArr[0].transform;
        transform2.localScale = _originalScale;
        transform3.localScale = _originalScaleShadow;
        
        // Сброс вращения
        transform2.localRotation = Quaternion.identity;
        transform3.localRotation = Quaternion.identity;
        
        // Ensure color is reset to original
        PropertyBlock.SetColor(_colorProperty, OriginalColor);
        ApplyPropertyView();
    }

    public void CritColorEffect()
    {
        // Reset color to original before starting the crit effect
        PropertyBlock.SetColor(_colorProperty, OriginalColor);
        ApplyPropertyView();
        
        if (_critColorRoutine != null) StopCoroutine(_critColorRoutine);
        _critColorRoutine = StartCoroutine(CritColorCoroutine());
    }
    
    public IEnumerator CritColorCoroutine()
    {
        PropertyBlock.SetColor(_colorProperty, Color.red);
        ApplyPropertyView();
        yield return new WaitForSeconds(duration);
        PropertyBlock.SetColor(_colorProperty, OriginalColor);
        ApplyPropertyView();
    }

    public void SetSortingOrder(SortingOrderData data)
    {
        foreach (var x in borderRenderArr) x.sortingOrder = data.ramka;
        foreach (var x in viewRenderArr) x.sortingOrder = data.view;
        foreach (var x in shadowRenderArr) x.sortingOrder = data.shadow;
        foreach (var x in backRenderArr) x.sortingOrder = data.back;
    }

    public IEnumerator DeathAnimation()
    {
        var transform1 = viewRenderArr[0].transform;
        var transform2 = shadowRenderArr[0].transform;
        
        StartCoroutine(GameAnimation.ShrinkAndDisappear(transform1, transform1.localScale, deathAnimDuration));
        yield return StartCoroutine(GameAnimation.ShrinkAndDisappear(transform2, transform2.localScale, deathAnimDuration));
    }

    public void SetFullColor(Color color, Color borderColor)
    {
        // OriginalColor = color;
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
        PropertyBlockRamka.SetColor(borderColorProp, OriginalRamkaColor);
        foreach (var x in borderRenderArr) x.SetPropertyBlock(PropertyBlockRamka);
    }

    public void ApplyBorderColor(Color black)
    {
        OriginalRamkaColor = black;
        ApplyBorderColor();
    }
    
    public virtual void SetSprites(EnemyData enemyData)
    {
        if(blockSpriteReplace)
            return;
        
        viewRenderArr[0].sprite = enemyData.spriteView;
        borderRenderArr[0].sprite = enemyData.spriteBorder;
        backRenderArr[0].sprite = enemyData.spriteView;
        shadowRenderArr[0].sprite = enemyData.spriteView;
    }

    public void SetDirection(Vector2 moveDirection)
    {
        _originalScale.x *=moveDirection.x < 0 ? -1 : 1;
        _originalScaleShadow.x *= moveDirection.x < 0 ? -1 : 1;

        viewRenderArr[0].transform.localScale = _originalScale;
        shadowRenderArr[0].transform.localScale = _originalScaleShadow;
    }
}