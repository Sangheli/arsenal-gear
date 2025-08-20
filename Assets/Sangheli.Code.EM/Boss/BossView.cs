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
        
        render.OriginalColor = color;
        render.ApplyPropertyView(color);
        render.ApplyPropertyBack(backColor);
        render.ApplyBorderColor(ramkaColor);
        deathVfx.SetColor(color);
        
        render.SetOriginalScale(Vector3.one);
        deathVfx.SetScale(Vector3.one/2f);
    }

    public override void FlashEffect(bool isCrit = false)
    {
        boss2Render.FlashEffect(isCrit);
        base.FlashEffect(isCrit);
    }

    public override void ResetItem()
    {
        boss2Render.ResetItem();
        base.ResetItem();
    }
}