using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    public Text timerText;
    public float punchScale = 1.2f;
    public float duration = 0.2f;
    private Color _startColor;
    private CancellationTokenSource _bounceCancellation;
    private Vector3 _originalScale;
    private RectTransformShake _shake;
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

    public bool Show(float hp, float fullValue, float fillValue, bool doBlink, bool startValue)
    {
        if (!gameObject.activeSelf)
            return false;
        
        foreach (var x in halfBarViewArr) x.Show(fillValue, doBlink, useStartValue: startValue);

        if (timerText == null) 
            return true;
        
        timerText.text = $"{NumberFormatter.Format(hp, 1)}/{NumberFormatter.Format(fullValue, 1)}";
        if (!doBlink) 
            return true;
        
        CancelBounce();
        if(gameObject.activeSelf && timerText.gameObject.activeSelf)
        {
            _bounceCancellation = new CancellationTokenSource();
            _ = GameAnimation.Bounce(timerText.transform, _originalScale, punchScale, duration, cancellationToken: _bounceCancellation.Token).LogExceptions();
        }

        return true;
    }

    public void Shake(ShakeSettings settings)
    {
        if (!gameObject.activeSelf)
            return;

        if (_shake == null)
        {
            _shake = GetComponent<RectTransformShake>();
            if (_shake == null)
                _shake = gameObject.AddComponent<RectTransformShake>();
        }

        _shake.Shake(settings, addRandomness: true);
    }

    private void CancelBounce()
    {
        if (_bounceCancellation == null)
            return;

        _bounceCancellation.Cancel();
        _bounceCancellation.Dispose();
        _bounceCancellation = null;
    }

    private void OnDestroy()
    {
        CancelBounce();
    }
}
