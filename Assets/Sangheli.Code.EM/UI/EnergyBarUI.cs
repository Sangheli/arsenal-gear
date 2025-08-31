using UnityEngine;
using UnityEngine.UI;

public class EnergyBarUI : MonoBehaviour
{
    public Text energyText;
    public float punchScale = 1.2f;
    public float duration = 0.2f;

    private Coroutine _bounceCoroutine;
    private Vector3 _originalScale;

    private void Awake()
    {
        _originalScale = energyText.transform.localScale;
    }

    public void ShowEnergy(int value)
    {
        if(!gameObject.activeSelf)
            return;
        
        energyText.text = $"{NumberFormatter.Format(value, 0)}";
        if (_bounceCoroutine != null) StopCoroutine(_bounceCoroutine);
        _bounceCoroutine = StartCoroutine(GameAnimation.Bounce(energyText.transform, _originalScale, punchScale, duration));
    }
} 