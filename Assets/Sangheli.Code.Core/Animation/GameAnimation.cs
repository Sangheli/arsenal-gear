using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine.UI;

public static class GameAnimation
{
    private const float EndWaitTime = 0.25f;
    public static readonly int PropInnerRadius = Shader.PropertyToID("_InnerRadius");
    
    public static async UniTask Bounce(Transform view, Vector3 originalScale, float punchScale, float duration, bool doNotChangeScaleOnStart = false, CancellationToken cancellationToken = default)
    {
        if(!doNotChangeScaleOnStart)
            view.localScale = originalScale;
        
        var halfDuration = duration / 2f;
        var targetScale = originalScale * punchScale;

        await OnScaleAnim(view, halfDuration, originalScale, targetScale, cancellationToken);
        await OnScaleAnim(view, halfDuration, targetScale, originalScale, cancellationToken);
        
        view.localScale = originalScale;
    }
    
    public static async UniTask OnScaleAnim(Transform transform, Vector3 originalScale, float duration, float scale1, float scale2, CancellationToken cancellationToken = default)
    {
        var t = 0f;
        while (t < duration)
        {
            var progress = t / duration;
            transform.localScale = originalScale * Mathf.Lerp(scale1, scale2, progress);;
            await UniTask.Yield();
            cancellationToken.ThrowIfCancellationRequested();
            t += Time.deltaTime;
        }
    }
    
    public static async UniTask BlinkColor(Color color, Color startColor, Image image, float blinkTime, AnimationCurve blinkCurve, CancellationToken cancellationToken = default)
    {
        image.color = color;
        var time = 0f;
        
        while (time < blinkTime)
        {
            var t = time / blinkTime;
            var curveValue = blinkCurve.Evaluate(t);
            image.color = Color.Lerp(color, startColor, curveValue);
            await UniTask.Yield();
            cancellationToken.ThrowIfCancellationRequested();
            time += Time.deltaTime;
        }

        image.color = startColor;
    }

    public static async UniTask ShrinkAndDisappear(Transform view, Vector3 scale1, float duration, CancellationToken cancellationToken = default)
    {
        await OnScaleAnim(view, duration, scale1, new Vector3(0,0,1), cancellationToken);
        view.localScale = Vector3.zero;
    }
    
    public static async UniTask FlashEffect(System.Action<float> setHitValue, bool isCrit, float maxFlashValue, float duration, CancellationToken cancellationToken = default)
    {
        setHitValue(isCrit ? maxFlashValue * 2f : maxFlashValue);
        
        var t = 0f;
        // Плавное затухание
        while (t < duration)
        {
            t += Time.deltaTime;
            var blend = Mathf.Lerp(isCrit ? maxFlashValue * 2f : maxFlashValue, 0f, t / duration);
            setHitValue(blend);
            await UniTask.Yield();
            cancellationToken.ThrowIfCancellationRequested();
        }

        setHitValue(0f);
    }

    public static async UniTask PulseInnerRadius(Renderer viewRender, MaterialPropertyBlock propertyBlock, float duration, float baseValue, CancellationToken cancellationToken = default)
    {
        var time = 0f;
        while (time < duration)
        {
            var t = time / duration;
            var radius = Mathf.Lerp(baseValue, 1f, t);

            propertyBlock.SetFloat(PropInnerRadius, radius);
            viewRender.SetPropertyBlock(propertyBlock);

            time += Time.deltaTime;
            await UniTask.Yield();
            cancellationToken.ThrowIfCancellationRequested();
        }
        
        propertyBlock.SetFloat(PropInnerRadius, baseValue);
        viewRender.SetPropertyBlock(propertyBlock);
    }
    
    private static async UniTask OnScaleAnim(Transform view, float duration, Vector3 scale1, Vector3 scale2, CancellationToken cancellationToken = default)
    {
        var t = 0f;
        while (t < duration)
        {
            var progress = t / duration;
            view.localScale = Vector3.Lerp(scale1, scale2, progress);
            await UniTask.Yield();
            cancellationToken.ThrowIfCancellationRequested();
            t += Time.deltaTime;
        }
    }
    
    public static async UniTask CollectAllEnergyOnScreen<T>(List<T> toCollect, int countToCollect, CancellationToken cancellationToken = default) where T : ICollectable
    {
        if (toCollect == null || toCollect.Count == 0) 
            return;
        
        var selectedToCollect = toCollect.OrderBy(_ => UnityEngine.Random.value).Take(countToCollect).ToList();

        var player = G.I.playerCursor.transform;
        
        foreach (var x in selectedToCollect)
        {
            x.IsActive = true;
            x.Collect(player, 0.1f,0.1f);
        }
        
        float elapsed = 0f;
        const float timeout = 5f;
        while (selectedToCollect.Any(x => x.IsActive) && elapsed < timeout)
        {
            elapsed += Time.deltaTime;
            await UniTask.Yield();
            cancellationToken.ThrowIfCancellationRequested();
        }

        await UniTask.Delay(TimeSpan.FromSeconds(EndWaitTime), cancellationToken: cancellationToken);
    }
}