using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    public Text timerText;
    public float punchScale = 1.2f;
    public float duration = 0.2f;
    private Color _startColor;
    private Coroutine _blinkCoroutine;
    private Coroutine _bounceCoroutine;
    private Vector3 _originalScale;
    public float unFillTime = 0.5f;

    public HalfBarView[] halfBarViewArr;

    private void Awake()
    {
        if (timerText != null)
            _originalScale = timerText.transform.localScale;
    }

    public void Init()
    {
        foreach (var x in halfBarViewArr) x.UnFillTime = unFillTime;
    }

    public bool Show(float hp, float fullValue, float fillValue, bool doBlink)
    {
        if (!gameObject.activeSelf)
            return false;
        
        foreach (var x in halfBarViewArr) x.Show(fillValue, doBlink);

        if (timerText == null) 
            return true;
        
        timerText.text = $"{NumberFormatter.Format(hp, 1)}/{NumberFormatter.Format(fullValue, 1)}";
        if (!doBlink) 
            return true;
        
        if (_bounceCoroutine != null) StopCoroutine(_bounceCoroutine);
        _bounceCoroutine = StartCoroutine(GameAnimation.Bounce(timerText.transform, _originalScale, punchScale, duration));

        return true;
    }
} 