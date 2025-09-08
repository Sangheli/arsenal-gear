using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CockpitManager : MonoBehaviour
{
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

    private void Awake()
    {
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
        // 1. distortionValueStart -> distortionValueDamage
        yield return AnimateDistortion(distortionValueStart, distortionValueDamage, distortionTime,glowValueStart,glowValueDamage);
        
        // 2. distortionValueDamage -> distortionValueStart
        yield return AnimateDistortion(distortionValueDamage, distortionValueStart, distortionTime2, glowValueDamage, glowValueStart);
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