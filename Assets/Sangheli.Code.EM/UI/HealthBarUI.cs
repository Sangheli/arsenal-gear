using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HealthBarUI : MonoBehaviour
{
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

    public HalfBarView[] halfBarViewArr;

    private void Awake()
    {
        if (timerText != null)
            _originalScale = timerText.transform.localScale;
    }

    public void Init()
    {
        foreach (var x in halfBarViewArr)
        {
            x.Init();
            x.BlinkColor = blinkColor;
            x.BlinkTime = blinkTime;
            x.BlinkCurve = blinkCurve;
        }
    }

    public void SetStartColor()
    {
        foreach (var x in halfBarViewArr)
            x.SetStartColor();
    }

    public bool ShowHp(float hp, float fullValue, float fillValue, bool doBlink)
    {
        if (!gameObject.activeSelf)
            return false;
        
        foreach (var x in halfBarViewArr) x.ShowHp(fillValue, doBlink);

        if (timerText != null)
        {
            timerText.text = $"{NumberFormatter.Format(hp, 1)}/{NumberFormatter.Format(fullValue, 1)}";
            if (doBlink)
            {
                if (_bounceCoroutine != null) StopCoroutine(_bounceCoroutine);
                _bounceCoroutine = StartCoroutine(GameAnimation.Bounce(timerText.transform, _originalScale, punchScale, duration));
            }
        }

        return true;
    }
} 