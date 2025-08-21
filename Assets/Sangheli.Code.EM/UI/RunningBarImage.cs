using UnityEngine;
using UnityEngine.UI;

public class RunningBarImage : ARunningBar
{
    //---------------------------
    public Image hpImage;
    public Image hpImageWhiteBack;
    public Image hpImageBack;
    
    private Material _hpImageMat;
    private Material _hpImageWhiteBackMat;
    private Material _hpImageBackMat;

    private void Awake()
    {
        _hpImageMat = GetMat(hpImage);
        _hpImageWhiteBackMat = GetMat(hpImageWhiteBack);
        _hpImageBackMat = GetMat(hpImageBack);
    }

    private static Material GetMat(Image img)
    {
        if (img.material != null)
            img.material = Instantiate(img.material);

        return img.material;
    }
    
    public override void UpdateView()
    {
        _hpImageMat.SetFloat(RunningBarValue.ClipUvLeft, RunningBarValue.ClipplingLeftFill);
        _hpImageMat.SetFloat(RunningBarValue.ClipUvRight, RunningBarValue.ClipplingRightFill);
        
        _hpImageWhiteBackMat.SetFloat(RunningBarValue.ClipUvLeft, RunningBarValue.ClipplingLeftWhite);
        _hpImageWhiteBackMat.SetFloat(RunningBarValue.ClipUvRight, RunningBarValue.ClipplingRightWhite);
        
        _hpImageBackMat.SetFloat(RunningBarValue.ClipUvLeft, RunningBarValue.ClipplingLeftBack);
        _hpImageBackMat.SetFloat(RunningBarValue.ClipUvRight, RunningBarValue.ClipplingRightBack);
    }

    public override void SetCurrentValue(float newValue)
    {
        _hpImageWhiteBackMat.SetFloat(RunningBarValue.ClipUvRight, 1 - newValue);
        _hpImageBackMat.SetFloat(RunningBarValue.ClipUvLeft, newValue);
    }
}