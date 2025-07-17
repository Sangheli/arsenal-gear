using UnityEngine;

public class BossView : EnemyView
{
    [Space]
    public Color bossColor = Color.red;

    public float colorMult = 0.8f;
    
    private void Start()
    {
        SetColor(bossColor);
    }

    public void SetScale(float viewScale)
    {
        scale = viewScale;
        view.transform.localScale = OriginalScale*scale;
        viewShadow.transform.localScale = OriginalScaleShadow*scale;
        deathVfx.SetScale(viewScale);
    }

    public void SetColor(Color color)
    {
        color *= colorMult;
        color.a = 1;
        
        var backColor = color;
        backColor.a = 64 / 255f;
        PropertyBlockBack.SetColor(ColorProperty, backColor);
        ApplyPropertyBack();
        
        deathVfx.SetColor(color);
		bossColor = color;
        ApplyPropertyBlock();
    }

    protected override void ApplyPropertyBlock()
    {
        PropertyBlock.SetColor(ColorProperty, bossColor);
        base.ApplyPropertyBlock();
    }
} 