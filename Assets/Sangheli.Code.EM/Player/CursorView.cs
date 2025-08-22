using UnityEngine;
using System.Collections;

public class CursorView : MonoBehaviour
{
    public GameObject view;
    public GameObject viewHarvest;
    public Renderer viewRender;
    public float punchScale = 1.2f;
    public float duration = 0.2f;
    public float durationVfx = 1f;
    private Vector3 _originalScale;
    private Vector3 _originalScaleVfx;
    private static readonly int RingThickness = Shader.PropertyToID("_RingThickness");
    private static readonly int InnerColor = Shader.PropertyToID("_InnerColor");
    private static readonly int InnerColor2 = Shader.PropertyToID("_InnerColor2");
    private static readonly int RingColor = Shader.PropertyToID("_RingColor");
    public MaterialPropertyBlock PropertyBlock;
    public float baseRingThickness = 0.25f;
    public Color color;
    public Color color2 = new Color(0,0,0,122f/256f);
    public CursorHudView hudView;
    public Color originalInnerColorRingOuter = Color.white;
    private Coroutine _pulseCoroutine;
    private float _currentScale = 1f;
    public bool isPulsing;
    private Color _pulseColor;
    public DeathVfx deathVfx;
    public GameObject dot;
    
    private void Awake()
    {
        _originalScale = view.transform.localScale;
        _originalScaleVfx = deathVfx.transform.localScale;
        PropertyBlock = new MaterialPropertyBlock();
        viewRender.GetPropertyBlock(PropertyBlock);
    }

    public void SetScale(float damageScale, float harvestScale)
    {
        _currentScale = damageScale;
        view.transform.localScale = _originalScale * damageScale;
        viewHarvest.transform.localScale = _originalScale * ( harvestScale);
        deathVfx.SetScale(damageScale);
        ApplyVisual();
    }

    public void Toggle(bool state)
    {
        view.gameObject.SetActive(state);
        viewHarvest.gameObject.SetActive(false);
        dot.gameObject.SetActive(false);
        
        if (state)
        {
            isPulsing = false;
            ApplyVisual();
        }
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
        
        isPulsing = false;
        ApplyVisual();

        _pulseColor = pulseColor;
        _pulseCoroutine = StartCoroutine(PulseRingColorCoroutine(pulseColor, duration));
    }

    private IEnumerator PulseRingColorCoroutine(Color pulseColor, float duration)
    {
        isPulsing = false;
        ApplyVisual();

        isPulsing = true;
        float t = 0f;
        while (t < duration)
        {
            float lerp = t / duration;
            
            _pulseColor = Color.Lerp(originalInnerColorRingOuter, pulseColor, lerp);
            ApplyVisual();
            
            t += Time.deltaTime;
            yield return null;
        }
        
        _pulseColor = pulseColor;
        ApplyVisual();

        t = 0f;
        float fadeDuration = duration * 2f;
        while (t < fadeDuration)
        {
            float lerp = t / fadeDuration;
            
            _pulseColor = Color.Lerp(pulseColor, originalInnerColorRingOuter, lerp);
            ApplyVisual();
            
            t += Time.deltaTime;
            yield return null;
        }
        
        isPulsing = false;
        ApplyVisual();
    }

    public void ApplyVisual()
    {
        PropertyBlock.SetFloat(RingThickness, baseRingThickness / _currentScale);
        PropertyBlock.SetColor(RingColor, isPulsing ? _pulseColor : originalInnerColorRingOuter);
        
        PropertyBlock.SetColor(InnerColor, color);
        PropertyBlock.SetColor(InnerColor2, color2);
        
        viewRender.SetPropertyBlock(PropertyBlock);
    }

    public void PlayDeathAnimation()
    {
        G.I.Container.cameraShake.DeathShake();
        G.I.Container.pooledAudioPlayer.PlayWithRandomPitch(G.I.Data.soundContainer.emptyHit);
        deathVfx.Play();
    }
} 