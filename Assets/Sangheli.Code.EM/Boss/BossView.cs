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
        _originalScale *= viewScale;
        _originalScaleShadow *= viewScale;
        _originalScaleShadow *= viewScale;
        view.transform.localScale = _originalScale;
        viewShadow.transform.localScale = _originalScaleShadow;
        deathVfx.transform.localScale = _originalScaleShadow;
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