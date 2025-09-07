using UnityEngine;
using UnityEngine.UI;

public class RunningBarImage : ARunningBar
{
    public Image hpImage;
    private Material _hpImageBlock;

    private void Awake()
    {
        _hpImageBlock = GetMat(hpImage);
    }

    private static Material GetMat(Image img)
    {
        if (img.material != null)
            img.material = Instantiate(img.material);

        return img.material;
    }
    
    public override void UpdateView()
    {
        _hpImageBlock.SetFloat(RunningBarValue.FillX, RunningBarValue.HpFill);
        _hpImageBlock.SetFloat(RunningBarValue.FillY, RunningBarValue.WhiteFill);
    }

    public override void SetCurrentValue(float newValue)
    {
        _hpImageBlock.SetFloat(RunningBarValue.FillX, RunningBarValue.HpFill);
        _hpImageBlock.SetFloat(RunningBarValue.FillY, newValue);
    }

    public override void SetSortingOrder(int order)
    {
    }

    public override void SetColor(Color fill, Color back)
    {
    }
}