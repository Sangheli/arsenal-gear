using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CockpitManager : MonoBehaviour
{
    public Transform shakeView;
    public float shakeForce;
    public float shakeTime;
    
    [Space]
    public float distortionTime;
    public float distortionTime2;
    public float distortionValueStart;
    public float distortionValueDamage;
    public float glowValueStart;
    public float glowValueDamage;
    
    [Space]
    public Image[] shieldImageArr;
    private readonly List<Material> _imageBlockArr = new();

    [Space]
    public string propName;
    public string propNameGlow;

    private int _distortion;
    private int _glow;

    private Vector3 _startShakeViewPos;

    private void Awake()
    {
        _startShakeViewPos = shakeView.localPosition;
        _distortion = Shader.PropertyToID(propName);
        _glow = Shader.PropertyToID(propNameGlow);
        foreach (var x in shieldImageArr) _imageBlockArr.Add(RunningBarImage.GetMat(x));
    }

    public void PlayDistortion()
    {
        StopAllCoroutines();
        StartCoroutine(DistortionRoutine());
    }

    private IEnumerator DistortionRoutine()
    {
        DoShake();
        // 1. distortionValueStart -> distortionValueDamage
        yield return AnimateDistortion(distortionValueStart, distortionValueDamage, distortionTime,glowValueStart,glowValueDamage);
        
        // 2. distortionValueDamage -> distortionValueStart
        yield return AnimateDistortion(distortionValueDamage, distortionValueStart, distortionTime2, glowValueDamage, glowValueStart);
    }

    private void DoShake()
    {
        // Сначала сбрасываем позицию
        shakeView.localPosition = _startShakeViewPos;
        // Запускаем корутину тряски
        StartCoroutine(ShakeRoutine());
    }

    private IEnumerator ShakeRoutine()
    {
        float elapsed = 0f;

        while (elapsed < shakeTime)
        {
            elapsed += Time.deltaTime;
        
            // Смещение по X/Y (можно расширить до Z)
            float offsetX = Random.Range(-1f, 1f) * shakeForce;
            float offsetY = Random.Range(-1f, 1f) * shakeForce;

            shakeView.localPosition = _startShakeViewPos + new Vector3(offsetX, offsetY, 0);

            yield return null;
        }

        // Возврат в стартовую позицию
        shakeView.localPosition = _startShakeViewPos;
    }

    private IEnumerator AnimateDistortion(float from, float to, float duration, float fromGlow, float toGlow)
    {
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float value = Mathf.Lerp(from, to, t / duration);
            float valueGlow = Mathf.Lerp(fromGlow, toGlow, t / duration);

            foreach (var mat in _imageBlockArr) mat.SetFloat(_distortion, value);
            foreach (var mat in _imageBlockArr) mat.SetFloat(_glow, valueGlow);

            yield return null;
        }

        // финальное значение
        foreach (var mat in _imageBlockArr) mat.SetFloat(_distortion, to);
        foreach (var mat in _imageBlockArr) mat.SetFloat(_glow, toGlow);
    }
}