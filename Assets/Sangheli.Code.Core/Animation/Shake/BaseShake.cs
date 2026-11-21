using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public abstract class BaseShake<T> : MonoBehaviour
{
    [Header("Shake Settings")]
    public ShakeSettings settings;

    protected T Target;
    private Vector2 _originalPosition;
    private CancellationTokenSource _shakeCancellation;

    protected abstract Vector2 GetPosition();
    protected abstract void SetPosition(Vector2 pos);

    protected virtual void Awake()
    {
        Target = GetComponent<T>();
        _originalPosition = GetPosition();
    }

    public void Shake(ShakeSettings shakeSettings, bool addRandomness = false)
    {
        _ = ShakeAsync(shakeSettings, addRandomness).LogExceptions();
    }

    public async UniTask ShakeAsync(ShakeSettings shakeSettings, bool addRandomness = false)
    {
        CancelShake();
        var cancellation = new CancellationTokenSource();
        _shakeCancellation = cancellation;
        var token = cancellation.Token;

        try
        {
            SetPosition(_originalPosition);
            var elapsed = 0f;

            while (elapsed < shakeSettings.duration)
            {
                var progress = Mathf.Clamp01(elapsed / shakeSettings.duration);
                var curveValue = shakeSettings.curve.Evaluate(progress);
                var x = Mathf.Sin(Time.time * shakeSettings.frequency) * shakeSettings.intensity * curveValue;
                var y = Mathf.Cos(Time.time * shakeSettings.frequency * 0.8f) * shakeSettings.intensity * curveValue;

                if (addRandomness)
                {
                    x += Random.Range(-0.1f, 0.1f) * shakeSettings.intensity * curveValue;
                    y += Random.Range(-0.1f, 0.1f) * shakeSettings.intensity * curveValue;
                }

                SetPosition(_originalPosition + new Vector2(x, y));
                elapsed += Time.deltaTime;
                await UniTask.Yield();
                token.ThrowIfCancellationRequested();
            }
        }
        finally
        {
            if (_shakeCancellation == cancellation)
            {
                _shakeCancellation = null;
                cancellation.Dispose();
                if (this != null) SetPosition(_originalPosition);
            }
        }
    }

    public void StopShake()
    {
        if (_shakeCancellation == null)
            return;

        CancelShake();
        SetPosition(_originalPosition);
    }

    private void CancelShake()
    {
        if (_shakeCancellation == null)
            return;

        var cancellation = _shakeCancellation;
        _shakeCancellation = null;
        cancellation.Cancel();
        cancellation.Dispose();
    }

    private void OnDestroy()
    {
        CancelShake();
    }
}
