using System.Collections;
using UnityEngine;

public class SaveInfo : MonoBehaviour
{
    public CanvasGroup canvasGroup;
    private Coroutine _fadeCoroutine;

    public void Show()
    {
        canvasGroup.alpha = 1f;
    }
    
    public void FadeOut(float duration,float awaitTIme)
    {
        if (_fadeCoroutine != null)
            StopCoroutine(_fadeCoroutine);
        _fadeCoroutine = StartCoroutine(FadeOutCoroutine(duration, awaitTIme));
    }
    
    private IEnumerator FadeOutCoroutine(float duration, float awaitTIme)
    {
        yield return new WaitForSeconds(awaitTIme);
        float startAlpha = 1;
        float time = 0f;
        while (time < duration)
        {
            time += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, 0f, time / duration);
            yield return null;
        }
        canvasGroup.alpha = 0f;
    }
}