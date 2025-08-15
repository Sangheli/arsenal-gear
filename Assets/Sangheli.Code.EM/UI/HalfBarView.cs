using UnityEngine;
using UnityEngine.UI;

public class HalfBarView : MonoBehaviour
{
    public Image hpImage;
    public Image hpImageBack;
    
    public Color BlinkColor { get; set; }
    public float BlinkTime  { get; set; }
    public AnimationCurve BlinkCurve { get; set; }
    
    private Color _startColor;
    private Coroutine _blinkCoroutine;
    private Coroutine _bounceCoroutine;
    
    private void Awake()
    {
        _startColor = hpImage.color;
    }

    public void Init()
    {
        hpImage.color = _startColor;
    }
    
    public void SetStartColor()
    {
        hpImage.color = _startColor;
    }
    
    public bool ShowHp(float fillValue, bool doBlink)
    {
        if (!gameObject.activeSelf)
            return false;

        hpImage.fillAmount = fillValue;
        hpImageBack.fillAmount = 1 - fillValue;
        
        if (doBlink && hpImage != null)
        {
            if (_blinkCoroutine != null)
                StopCoroutine(_blinkCoroutine);
            _blinkCoroutine = StartCoroutine(GameAnimation.BlinkColor(BlinkColor, _startColor, hpImage, BlinkTime, BlinkCurve));
        }

        return true;
    }
}
