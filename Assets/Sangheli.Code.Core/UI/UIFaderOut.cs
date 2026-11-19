using System.Collections;
using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class UIFaderOut : MonoBehaviour
{
    public CanvasGroup canvasGroup;
    private CancellationTokenSource _fadeCancellation;

    public void Show()
    {
        canvasGroup.alpha = 1f;
    }
    
    public void FadeOut(float duration,float awaitTIme)
    {
        CancelFade();
        _fadeCancellation = new CancellationTokenSource();
        _ = FadeOutAsync(duration, awaitTIme, _fadeCancellation.Token);
    }
    
    private async Task FadeOutAsync(float duration, float awaitTIme, CancellationToken cancellationToken)
    {
        await Task.Delay(TimeSpan.FromSeconds(awaitTIme), cancellationToken);
        float startAlpha = 1;
        float time = 0f;
        while (time < duration)
        {
            time += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, 0f, time / duration);
            await Task.Yield();
            cancellationToken.ThrowIfCancellationRequested();
        }
        canvasGroup.alpha = 0f;
    }

    private void CancelFade()
    {
        if (_fadeCancellation == null)
            return;

        _fadeCancellation.Cancel();
        _fadeCancellation.Dispose();
        _fadeCancellation = null;
    }

    private void OnDestroy()
    {
        CancelFade();
    }
}
