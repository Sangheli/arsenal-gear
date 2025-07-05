using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour
{
    [Header("Shake Settings")]
    public CameraShakeSettings settings;
    
    [Header("Override Settings (if settings asset is not assigned)")]
    public float shakeIntensity = 0.3f;
    public float shakeDuration = 0.15f;
    public float shakeFrequency = 80f;
    public AnimationCurve shakeCurve = AnimationCurve.EaseInOut(0, 1, 1, 0);
    
    [Header("Death Shake Settings")]
    public float deathShakeIntensity = 0.8f;
    public float deathShakeDuration = 0.25f;
    public float deathShakeFrequency = 120f;
    
    private Vector3 _originalPosition;
    private bool _isShaking = false;
    private Coroutine _shakeCoroutine;
    
    private void Awake()
    {
        _originalPosition = transform.localPosition;
    }
    
    public void Shake()
    {
        if (_isShaking)
        {
            StopCoroutine(_shakeCoroutine);
        }
        
        _shakeCoroutine = StartCoroutine(ShakeCoroutine());
    }
    
    public void Shake(float intensity, float duration)
    {
        if (_isShaking)
        {
            StopCoroutine(_shakeCoroutine);
        }
        
        _shakeCoroutine = StartCoroutine(ShakeCoroutine(intensity, duration));
    }
    
    public void DeathShake()
    {
        if (_isShaking)
        {
            StopCoroutine(_shakeCoroutine);
        }
        
        _shakeCoroutine = StartCoroutine(DeathShakeCoroutine());
    }
    
    private IEnumerator ShakeCoroutine(float intensity = -1f, float duration = -1f)
    {
        _isShaking = true;
        
        float actualIntensity = intensity > 0 ? intensity : (settings != null ? settings.shakeIntensity : shakeIntensity);
        float actualDuration = duration > 0 ? duration : (settings != null ? settings.shakeDuration : shakeDuration);
        float actualFrequency = settings != null ? settings.shakeFrequency : shakeFrequency;
        AnimationCurve actualCurve = settings != null ? settings.shakeCurve : shakeCurve;
        
        float elapsed = 0f;
        
        while (elapsed < actualDuration)
        {
            float progress = elapsed / actualDuration;
            float curveValue = actualCurve.Evaluate(progress);
            
            // Резкий шейк с высокой частотой для эффекта удара
            float x = Mathf.Sin(Time.time * actualFrequency) * actualIntensity * curveValue;
            float y = Mathf.Cos(Time.time * actualFrequency * 0.7f) * actualIntensity * curveValue;
            
            transform.localPosition = _originalPosition + new Vector3(x, y, 0);
            
            elapsed += Time.deltaTime;
            yield return null;
        }
        
        transform.localPosition = _originalPosition;
        _isShaking = false;
    }
    
    private IEnumerator DeathShakeCoroutine()
    {
        _isShaking = true;
        
        float actualIntensity = settings != null ? settings.deathShakeIntensity : deathShakeIntensity;
        float actualDuration = settings != null ? settings.deathShakeDuration : deathShakeDuration;
        float actualFrequency = settings != null ? settings.deathShakeFrequency : deathShakeFrequency;
        AnimationCurve actualCurve = settings != null ? settings.shakeCurve : shakeCurve;
        
        float elapsed = 0f;
        
        while (elapsed < actualDuration)
        {
            float progress = elapsed / actualDuration;
            float curveValue = actualCurve.Evaluate(progress);
            
            // Очень резкий шейк с высокой частотой для эффекта удара при смерти
            float x = Mathf.Sin(Time.time * actualFrequency) * actualIntensity * curveValue;
            float y = Mathf.Cos(Time.time * actualFrequency * 0.8f) * actualIntensity * curveValue;
            
            // Добавляем дополнительную случайность для более резкого эффекта
            x += Random.Range(-0.1f, 0.1f) * actualIntensity * curveValue;
            y += Random.Range(-0.1f, 0.1f) * actualIntensity * curveValue;
            
            transform.localPosition = _originalPosition + new Vector3(x, y, 0);
            
            elapsed += Time.deltaTime;
            yield return null;
        }
        
        transform.localPosition = _originalPosition;
        _isShaking = false;
    }
    
    public void StopShake()
    {
        if (_isShaking && _shakeCoroutine != null)
        {
            StopCoroutine(_shakeCoroutine);
            transform.localPosition = _originalPosition;
            _isShaking = false;
        }
    }
} 