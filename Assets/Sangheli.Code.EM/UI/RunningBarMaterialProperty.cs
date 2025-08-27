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
        _hpImageBlock.SetFloat(RunningBarValue.ClipUvLeft, RunningBarValue.ClipplingLeftFill);
        _hpImageBlock.SetFloat(RunningBarValue.ClipUvRight, RunningBarValue.ClipplingRightFill);
        hpImage.SetPropertyBlock(_hpImageBlock);
        
        _hpImageWhiteBackBlock.SetFloat(RunningBarValue.ClipUvLeft, RunningBarValue.ClipplingLeftWhite);
        _hpImageWhiteBackBlock.SetFloat(RunningBarValue.ClipUvRight, RunningBarValue.ClipplingRightWhite);
        hpImageWhiteBack.SetPropertyBlock(_hpImageWhiteBackBlock);
        
        _hpImageBackBlock.SetFloat(RunningBarValue.ClipUvLeft, RunningBarValue.ClipplingLeftBack);
        _hpImageBackBlock.SetFloat(RunningBarValue.ClipUvRight, RunningBarValue.ClipplingRightBack);
        hpImageBack.SetPropertyBlock(_hpImageBackBlock);
    }

    public override void SetCurrentValue(float newValue)
    {
        _hpImageWhiteBackBlock.SetFloat(RunningBarValue.ClipUvRight, 1 - newValue);
        hpImageWhiteBack.SetPropertyBlock(_hpImageWhiteBackBlock);
        
        _hpImageBackBlock.SetFloat(RunningBarValue.ClipUvLeft, newValue);
        hpImageBack.SetPropertyBlock(_hpImageBackBlock);
    }

    public override void SetSortingOrder(int order)
    {
        hpImage.sortingOrder = order;
        hpImageWhiteBack.sortingOrder = order;
        hpImageBack.sortingOrder = order;
    }
}