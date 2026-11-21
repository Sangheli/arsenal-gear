using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
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
    private CancellationTokenSource _distortionCancellation;
    private CancellationTokenSource _shakeCancellation;

    private void Awake()
    {
        _startShakeViewPos = shakeView.localPosition;
        _distortion = Shader.PropertyToID(propName);
        _glow = Shader.PropertyToID(propNameGlow);
        foreach (var x in shieldImageArr) _imageBlockArr.Add(RunningBarImage.GetMat(x));
    }

    public void PlayDistortion()
    {
        CancelAnimation(ref _distortionCancellation);
        CancelAnimation(ref _shakeCancellation);
        _distortionCancellation = new CancellationTokenSource();
        _ = DistortionAsync(_distortionCancellation.Token).LogExceptions();
    }

    private async UniTask DistortionAsync(CancellationToken cancellationToken)
    {
        DoShake();
        await AnimateDistortion(distortionValueStart, distortionValueDamage, distortionTime, glowValueStart, glowValueDamage, cancellationToken);
        await AnimateDistortion(distortionValueDamage, distortionValueStart, distortionTime2, glowValueDamage, glowValueStart, cancellationToken);
    }

    private void DoShake()
    {
        // Сначала сбрасываем позицию
        shakeView.localPosition = _startShakeViewPos;
        CancelAnimation(ref _shakeCancellation);
        _shakeCancellation = new CancellationTokenSource();
        _ = ShakeAsync(_shakeCancellation.Token).LogExceptions();
    }

    private async UniTask ShakeAsync(CancellationToken cancellationToken)
    {
        float elapsed = 0f;

        while (elapsed < shakeTime)
        {
            elapsed += Time.deltaTime;
        
            // Смещение по X/Y (можно расширить до Z)
            float offsetX = Random.Range(-1f, 1f) * shakeForce;
            float offsetY = Random.Range(-1f, 1f) * shakeForce;

            shakeView.localPosition = _startShakeViewPos + new Vector3(offsetX, offsetY, 0);

            await UniTask.Yield();
            cancellationToken.ThrowIfCancellationRequested();
        }

        // Возврат в стартовую позицию
        shakeView.localPosition = _startShakeViewPos;
    }

    private async UniTask AnimateDistortion(float from, float to, float duration, float fromGlow, float toGlow, CancellationToken cancellationToken)
    {
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float value = Mathf.Lerp(from, to, t / duration);
            float valueGlow = Mathf.Lerp(fromGlow, toGlow, t / duration);

            foreach (var mat in _imageBlockArr) mat.SetFloat(_distortion, value);
            foreach (var mat in _imageBlockArr) mat.SetFloat(_glow, valueGlow);

            await UniTask.Yield();
            cancellationToken.ThrowIfCancellationRequested();
        }

        // финальное значение
        foreach (var mat in _imageBlockArr) mat.SetFloat(_distortion, to);
        foreach (var mat in _imageBlockArr) mat.SetFloat(_glow, toGlow);
    }

    private static void CancelAnimation(ref CancellationTokenSource cancellation)
    {
        if (cancellation == null)
            return;

        cancellation.Cancel();
        cancellation.Dispose();
        cancellation = null;
    }

    private void OnDestroy()
    {
        CancelAnimation(ref _distortionCancellation);
        CancelAnimation(ref _shakeCancellation);
    }
}
