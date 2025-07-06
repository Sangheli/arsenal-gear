using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HealthBarUI : MonoBehaviour
{
    public Image hpImage;
    public Text timerText;
    public Color blinkColor;
    public float blinkTime = 0.05f;
    public AnimationCurve blinkCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    public float punchScale = 1.2f;
    public float duration = 0.2f;
    private Color _startColor;
    private Coroutine _blinkCoroutine;
    private Coroutine _bounceCoroutine;
    private Vector3 _originalScale;

    private void Awake()
    {
        if (hpImage != null)
            _startColor = hpImage.color;
        if (timerText != null)
            _originalScale = timerText.transform.localScale;
    }

    public void Init()
    {
        if (hpImage != null)
            hpImage.color = _startColor;
    }

    public void SetStartColor()
    {
        hpImage.color = _startColor;
    }

    public bool ShowHp(float hp, float fullValue, float fillValue, bool doBlink)
    {
        if (!gameObject.activeSelf)
            return false;

        if (hpImage != null)
            hpImage.fillAmount = fillValue;
        if (timerText != null)
        {
            timerText.text = $"{NumberFormatter.Format(hp, 1)}/{NumberFormatter.Format(fullValue, 1)}";
            if (doBlink)
            {
                if (_bounceCoroutine != null) StopCoroutine(_bounceCoroutine);
                _bounceCoroutine = StartCoroutine(GameAnimation.Bounce(timerText.transform, _originalScale, punchScale, duration));
            }
        }

        if (doBlink && hpImage != null)
        {
            if (_blinkCoroutine != null)
                StopCoroutine(_blinkCoroutine);
            _blinkCoroutine = StartCoroutine(GameAnimation.BlinkColor(blinkColor, _startColor, hpImage, blinkTime, blinkCurve));
        }

        return true;
    }
} 