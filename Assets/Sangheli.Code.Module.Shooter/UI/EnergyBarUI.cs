using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class EnergyBarUI : MonoBehaviour
{
    public Text energyText;
    public float punchScale = 1.2f;
    public float duration = 0.2f;

    private CancellationTokenSource _bounceCancellation;
    private Vector3 _originalScale;

    private void Awake()
    {
        _originalScale = energyText.transform.localScale;
    }

    public void ShowEnergy(int value)
    {
        if(!gameObject.activeSelf)
            return;
        
        energyText.text = $"{NumberFormatter.FormatAuto(value,1)}";
        CancelBounce();
        _bounceCancellation = new CancellationTokenSource();
        _ = GameAnimation.Bounce(energyText.transform, _originalScale, punchScale, duration, cancellationToken: _bounceCancellation.Token).LogExceptions();
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
