using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class BounceAnim : MonoBehaviour
{
    public float bounceScale = 1.2f;
    public float startBounceShift;
    public float bounceDuration = 0.15f;
    public bool splitDuration;

    public Vector3 baseScale;
    private CancellationTokenSource _bounceCancellation;

    private void Awake()
    {
        baseScale = transform.localScale;
    }

    public void ResetView()
    {
        CancelBounce();
        transform.localScale = baseScale;
    }

    public void Play()
    {
        _ = PlayAsync().LogExceptions();
    }

    public async Task PlayAsync()
    {
        if (!gameObject.activeSelf)
            return;

        CancelBounce();
        var cancellation = new CancellationTokenSource();
        _bounceCancellation = cancellation;
        var token = cancellation.Token;

        try
        {
            if (startBounceShift > 0)
                await Task.Delay(TimeSpan.FromSeconds(startBounceShift), token);

            var duration = splitDuration ? bounceDuration / 2f : bounceDuration;
            var targetScale = baseScale * bounceScale;
            await ScaleAsync(baseScale, targetScale, duration, token);
            await ScaleAsync(targetScale, baseScale, duration, token);
        }
        finally
        {
            if (_bounceCancellation == cancellation)
            {
                _bounceCancellation = null;
                cancellation.Dispose();
                if (this != null) transform.localScale = baseScale;
            }
        }
    }

    private async Task ScaleAsync(Vector3 startScale, Vector3 targetScale, float duration, CancellationToken token)
    {
        if (duration <= 0)
        {
            transform.localScale = targetScale;
            return;
        }

        var elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            transform.localScale = Vector3.Lerp(startScale, targetScale, Mathf.Clamp01(elapsed / duration));
            await Task.Yield();
            token.ThrowIfCancellationRequested();
        }

        transform.localScale = targetScale;
    }

    private void CancelBounce()
    {
        if (_bounceCancellation == null)
            return;

        var cancellation = _bounceCancellation;
        _bounceCancellation = null;
        cancellation.Cancel();
        cancellation.Dispose();
    }

    private void OnDestroy()
    {
        CancelBounce();
    }
}
