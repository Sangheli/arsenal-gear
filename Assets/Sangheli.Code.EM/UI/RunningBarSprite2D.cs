using UnityEngine;

public class RunningBarSprite2D : ARunningBar
{
    public SpriteRenderer hpImage;
    public SpriteRenderer blackBack;
    private MaterialPropertyBlock _hpImageBlock;

    private void Awake()
    {
        _hpImageBlock = new MaterialPropertyBlock();
        hpImage.GetPropertyBlock(_hpImageBlock);
    }

    public override void UpdateView()
    {
        _hpImageBlock.SetFloat(RunningBarValue.FillX, RunningBarValue.HpFill);
        _hpImageBlock.SetFloat(RunningBarValue.FillY, RunningBarValue.WhiteFill);
        hpImage.SetPropertyBlock(_hpImageBlock);
    }

    public override void SetCurrentValue(float newValue)
    {
        _hpImageBlock.SetFloat(RunningBarValue.FillX, RunningBarValue.HpFill);
        _hpImageBlock.SetFloat(RunningBarValue.FillY, newValue);
        hpImage.SetPropertyBlock(_hpImageBlock);
    }

    public override void SetSortingOrder(int order)
    {
        hpImage.sortingOrder = order;
        blackBack.sortingOrder = order-1;
    }

    public override void SetColor(Color fill, Color back)
    {
        hpImage.color = fill;
        // hpImageBack.color = back;
    }
}