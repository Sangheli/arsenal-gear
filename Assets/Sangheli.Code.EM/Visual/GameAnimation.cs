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
        var t = 0f;

        // Увеличение масштаба
        while (t < halfDuration)
        {
            t += Time.deltaTime;
            var progress = t / halfDuration;
            view.localScale = Vector3.Lerp(originalScale, targetScale, progress);
            yield return null;
        }

        t = 0f;

        // Возврат к исходному масштабу
        while (t < halfDuration)
        {
            t += Time.deltaTime;
            var progress = t / halfDuration;
            view.localScale = Vector3.Lerp(targetScale, originalScale, progress);
            yield return null;
        }

        view.localScale = originalScale;
    }
    
    public static IEnumerator OnScaleAnim(Transform transform, Vector3 originalScale, float t1, float scale1, float scale2)
    {
        var elapsedTime = 0f;
        while (elapsedTime < t1)
        {
            elapsedTime += Time.deltaTime;
            var progress = elapsedTime / t1;
            var currentScale = Mathf.Lerp(scale1, scale2, progress);
            transform.localScale = originalScale * currentScale;
            yield return null;
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
            time += Time.deltaTime;
            yield return null;
        }

        image.color = startColor;
    }

    public static IEnumerator ShrinkAndDisappear(Transform transform, Vector3 originalScale, float duration)
    {
        float timer = 0f;
        while (timer < duration)
        {
            float t = timer / duration;
            transform.localScale = Vector3.Lerp(originalScale, Vector3.zero, t);
            timer += Time.deltaTime;
            yield return null;
        }
        transform.localScale = Vector3.zero;
    }
    
    public static IEnumerator FlashEffect(System.Action<float> setHitValue, bool isCrit, float maxFlashValue, float fadeDuration)
    {
        setHitValue(isCrit ? maxFlashValue * 2f : maxFlashValue);
        
        var t = 0f;
        // Плавное затухание
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            var blend = Mathf.Lerp(isCrit ? maxFlashValue * 2f : maxFlashValue, 0f, t / fadeDuration);
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
}