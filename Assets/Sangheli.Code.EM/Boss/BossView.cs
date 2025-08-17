using System.Collections;
using UnityEngine;

public class BossView : EnemyView
{
    [Space]
    public float colorMult = 0.8f;

    public EnemyRender boss2Render;
    private Coroutine _bounceRoutineBoss2;
    private Coroutine _bounceShadowRoutineBoss;
    public Color ramkaColor = Color.black;

    public void SetScale(float viewScale)
    {
        render.SetScale(viewScale);
        boss2Render.shadowRender.transform.localPosition /= viewScale;
        deathVfx.SetScale(viewScale);
    }

    public void SetColor(Color color)
    {
        color *= colorMult;
        color.a = 1;
        
        var backColor = color;
        backColor.a = 64 / 255f;
        
        render.OriginalColor = color; // Store the original color for crit effects
        render.ApplyPropertyView(color);
        render.ApplyPropertyBack(backColor);
        render.ApplyBorderColor(ramkaColor);
        
        boss2Render.OriginalColor = color; // Store the original color for crit effects
        boss2Render.ApplyPropertyView(color);
        boss2Render.ApplyPropertyBack(backColor);
        
        deathVfx.SetColor(color);
    }

    public override void SetHitValue(float value)
    {
        boss2Render.SetHitValue(value);
        base.SetHitValue(value);
    }
    
    public override void SetFillValue(float targetClipValue)
    {
        boss2Render.SetFillValue(targetClipValue);
        base.SetFillValue(targetClipValue);
    }

    public override void FlashEffect(bool isCrit = false)
    {
        boss2Render.FlashEffect(isCrit);
        base.FlashEffect(isCrit);
    }

    public override void CritColorEffect()
    {
        boss2Render.CritColorEffect();
        base.CritColorEffect();
    }

    public override void ResetItem()
    {
        boss2Render.ResetItem();
        base.ResetItem();
    }
}