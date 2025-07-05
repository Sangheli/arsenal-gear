using UnityEngine;

public class BossView : EnemyView
{
    [Space]
    public Color bossColor = Color.red;

    private void Start()
    {
        SetColor(bossColor);
    }

    public void SetScale(float viewScale)
    {
        scale = viewScale;
        view.transform.localScale = _originalScale*scale;
        viewShadow.transform.localScale = _originalScaleShadow*scale;
        deathVfx.transform.localScale = _originalScaleVfx*scale;
    }

    public void SetColor(Color color)
    {
		bossColor = color;
        ApplyPropertyBlock();
    }

    protected override void ApplyPropertyBlock()
    {
        _propertyBlock.SetColor(ColorProperty, bossColor);
        base.ApplyPropertyBlock();
    }
} 