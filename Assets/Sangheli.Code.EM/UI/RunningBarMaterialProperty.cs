using UnityEngine;

public class RunningBarMaterialProperty : ARunningBar
{
    public Renderer hpImage;
    public Renderer hpImageWhiteBack;
    public Renderer hpImageBack;
    
    private MaterialPropertyBlock _hpImageBlock;
    private MaterialPropertyBlock _hpImageWhiteBackBlock;
    private MaterialPropertyBlock _hpImageBackBlock;

    private void Awake()
    {
        _hpImageBlock = new MaterialPropertyBlock();
        hpImage.GetPropertyBlock(_hpImageBlock);
        
        _hpImageWhiteBackBlock = new MaterialPropertyBlock();
        hpImageWhiteBack.GetPropertyBlock(_hpImageWhiteBackBlock);
        
        _hpImageBackBlock = new MaterialPropertyBlock();
        hpImageBack.GetPropertyBlock(_hpImageBackBlock);
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
        hpImageWhiteBack.sortingOrder = order;
        hpImageBack.sortingOrder = order;
    }

    public override void SetColor(Color fill, Color back)
    {
    }
}