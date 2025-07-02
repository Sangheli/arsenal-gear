using UnityEngine;
using System.Collections;

public class CursorView : MonoBehaviour
{
    public GameObject view;
    public GameObject viewHarvest;
    public Renderer viewRender;
    public float punchScale = 1.2f;
    public float duration = 0.2f;
    private Vector3 _originalScale;
    private static readonly int RingThickness = Shader.PropertyToID("_RingThickness");
    private static readonly int InnerColor = Shader.PropertyToID("_InnerColor");
    private static readonly int RingColor = Shader.PropertyToID("_RingColor");
    public MaterialPropertyBlock PropertyBlock;
    public float baseRingThickness = 0.25f;
    public Color color;
    public CursorHudView hudView;
    public Color originalInnerColorRingOuter = Color.white;
    private Coroutine _pulseCoroutine;
    private float _currentScale = 1f;
    private bool _isPulsing = false;
    private Color _pulseColor;

    private void Awake()
    {
        _originalScale = view.transform.localScale;
        PropertyBlock = new MaterialPropertyBlock();
        viewRender.GetPropertyBlock(PropertyBlock);
    }

    public void SetScale(float damageScale, float harvestScale)
    {
        _currentScale = damageScale;
        view.transform.localScale = _originalScale * damageScale;
        viewHarvest.transform.localScale = _originalScale * (damageScale + harvestScale);
        ApplyRingVisuals();
    }

    public void Toggle(bool state)
    {
        view.gameObject.SetActive(state);
        viewHarvest.gameObject.SetActive(false);
    }

    public void DoBounceAnimation(float scale)
    {
        var addonScale = punchScale - 1;
        addonScale /= scale;
        
        StopAllCoroutines();
        StartCoroutine(GameAnimation.Bounce(view.transform, _originalScale * scale, 1+addonScale, duration));
    }

    public void PulseRingColor(Color pulseColor, float duration)
    {
        if (_pulseCoroutine != null)
            StopCoroutine(_pulseCoroutine);
        _isPulsing = true;
        _pulseColor = pulseColor;
        ApplyRingVisuals();
        _pulseCoroutine = StartCoroutine(PulseRingColorCoroutine(pulseColor, duration));
    }

    private IEnumerator PulseRingColorCoroutine(Color pulseColor, float duration)
    {
        float t = 0f;
        while (t < duration)
        {
            float lerp = t / duration;
            _pulseColor = Color.Lerp(originalInnerColorRingOuter, pulseColor, lerp);
            ApplyRingVisuals();
            t += Time.deltaTime;
            yield return null;
        }
        _pulseColor = pulseColor;
        ApplyRingVisuals();

        t = 0f;
        float fadeDuration = duration * 2f;
        while (t < fadeDuration)
        {
            float lerp = t / fadeDuration;
            _pulseColor = Color.Lerp(pulseColor, originalInnerColorRingOuter, lerp);
            ApplyRingVisuals();
            t += Time.deltaTime;
            yield return null;
        }
        _isPulsing = false;
        ApplyRingVisuals();
    }

    private void ApplyRingVisuals()
    {
        PropertyBlock.SetFloat(RingThickness, baseRingThickness / _currentScale);
        PropertyBlock.SetColor(InnerColor, color);
        PropertyBlock.SetColor(RingColor, _isPulsing ? _pulseColor : originalInnerColorRingOuter);
        viewRender.SetPropertyBlock(PropertyBlock);
    }
} 