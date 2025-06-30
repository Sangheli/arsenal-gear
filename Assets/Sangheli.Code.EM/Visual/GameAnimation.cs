using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public static class GameAnimation
{
    public static IEnumerator Bounce(Transform view, Vector3 originalScale, float punchScale, float duration)
    {
        view.localScale = originalScale;
        var halfDuration = duration / 2f;
        var targetScale = originalScale * punchScale;

        yield return OnScaleAnim(view, halfDuration, originalScale, targetScale);
        yield return OnScaleAnim(view, halfDuration, targetScale, originalScale);
        
        view.localScale = originalScale;
    }
    
    public static IEnumerator OnScaleAnim(Transform transform, Vector3 originalScale, float duration, float scale1, float scale2)
    {
        var t = 0f;
        while (t < duration)
        {
            var progress = t / duration;
            transform.localScale = originalScale * Mathf.Lerp(scale1, scale2, progress);;
            yield return null;
            t += Time.deltaTime;
        }
    }
    
    public static IEnumerator BlinkColor(Color color, Color startColor, Image image, float blinkTime, AnimationCurve blinkCurve)
    {
        image.color = color;
        var time = 0f;
        
        while (time < blinkTime)
        {
            var t = time / blinkTime;
            var curveValue = blinkCurve.Evaluate(t);
            image.color = Color.Lerp(color, startColor, curveValue);
            yield return null;
            time += Time.deltaTime;
        }

        image.color = startColor;
    }

    public static IEnumerator ShrinkAndDisappear(Transform view, Vector3 scale1, float duration)
    {
        yield return OnScaleAnim(view, duration, scale1, Vector3.zero);
        view.localScale = Vector3.zero;
    }
    
    public static IEnumerator FlashEffect(System.Action<float> setHitValue, bool isCrit, float maxFlashValue, float duration)
    {
        setHitValue(isCrit ? maxFlashValue * 2f : maxFlashValue);
        
        var t = 0f;
        // Плавное затухание
        while (t < duration)
        {
            t += Time.deltaTime;
            var blend = Mathf.Lerp(isCrit ? maxFlashValue * 2f : maxFlashValue, 0f, t / duration);
            setHitValue(blend);
            yield return null;
        }

        setHitValue(0f);
    }

    public static IEnumerator AnimateDamageText(Text damageText, Vector3 originalScale, float duration, Color startColor, Color endColor)
    {
        damageText.color = startColor;
        float elapsed = 0f;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            var t = elapsed / duration;
            
            // Анимация масштабирования
            var scale = Mathf.Lerp(1.5f, 0f, t);
            damageText.transform.localScale = originalScale * scale;
            
            // Анимация цвета
            var currentColor = Color.Lerp(startColor, endColor, t);
            damageText.color = currentColor;
            
            yield return null;
        }
    }
    
    public static IEnumerator PulseInnerRadius(Renderer viewRender, MaterialPropertyBlock propertyBlock, float duration)
    {
        var time = 0f;
        while (time < duration)
        {
            var t = time / duration;
            var radius = Mathf.Lerp(0f, 1f, t);

            propertyBlock.SetFloat(Shader.PropertyToID("_InnerRadius"), radius);
            viewRender.SetPropertyBlock(propertyBlock);

            time += Time.deltaTime;
            yield return null;
        }
        
        propertyBlock.SetFloat(Shader.PropertyToID("_InnerRadius"), 0f);
        viewRender.SetPropertyBlock(propertyBlock);
    }
    
    private static IEnumerator OnScaleAnim(Transform view, float duration,Vector3 scale1, Vector3 scale2)
    {
        var t = 0f;
        while (t < duration)
        {
            var progress = t / duration;
            view.localScale = Vector3.Lerp(scale1, scale2, progress);
            yield return null;
            t += Time.deltaTime;
        }
    }
}