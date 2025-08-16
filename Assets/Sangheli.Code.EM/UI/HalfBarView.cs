using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HalfBarView : MonoBehaviour
{
    public Image hpImage;
    public Image hpImageWhiteBack;
    public Image hpImageBack;
    
    public Color BlinkColor { get; set; }
    public float BlinkTime  { get; set; }
    public AnimationCurve BlinkCurve { get; set; }
    
    private Color _startColor;
    private Coroutine _blinkCoroutine;
    private Coroutine _bounceCoroutine;
    public float Threshold { get; set; }
    
    public float UnFillTime { get; set; }
    
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
    
    public bool ShowHp(float fillValue, bool doBlink, bool doAnimatedUnfill)
    {
        if (!gameObject.activeSelf)
            return false;

        if(doAnimatedUnfill)
            StartCoroutine(AnimateFill(fillValue, UnFillTime));
        else
        {
            float targetValueWhite = fillValue;
            float targetValueBack = 1 - fillValue;
            hpImageWhiteBack.fillAmount = targetValueWhite;
            hpImageBack.fillAmount = targetValueBack;
        }

        hpImage.fillAmount = fillValue;
        
        if (doBlink && hpImage != null)
        {
            if (_blinkCoroutine != null)
                StopCoroutine(_blinkCoroutine);
            _blinkCoroutine =
                StartCoroutine(GameAnimation.BlinkColor(BlinkColor, _startColor, hpImage, BlinkTime, BlinkCurve));
        }

        return true;
    }

    //через апдейт проверять
    public IEnumerator AnimateFill(float newValue, float blinkTime)
    {
        float startValueWhite = hpImageWhiteBack.fillAmount;
        float startValueBack = hpImageBack.fillAmount;
        float targetValueWhite = newValue;
        float targetValueBack = 1 - newValue;
        
        float elapsed = 0f;
        while (elapsed < blinkTime)
        {
            elapsed += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsed / blinkTime);
            
            hpImageWhiteBack.fillAmount = Mathf.Lerp(startValueWhite, targetValueWhite, progress);
            hpImageBack.fillAmount = Mathf.Lerp(startValueBack, targetValueBack, progress);
            
            yield return null;
        }
        
        hpImageWhiteBack.fillAmount = targetValueWhite;
        hpImageBack.fillAmount = targetValueBack;
    }
}
