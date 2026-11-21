using UnityEngine;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScreenSpinManager : MonoBehaviour
{
    [Header("UI")] public RawImage screenImage;
    public RectTransform imageTransform;

    [Header("Animation Parameters")] public float duration = 2f;
    public float startRotationSpeed = 0f;
    public float endRotationSpeed = 1000f;
    public AnimationCurve scaleCurve = AnimationCurve.EaseInOut(0, 1, 1, 0);

    private Texture2D _screenshotTexture;
    private readonly CancellationTokenSource _destroyCancellation = new();

    private void OnValidate()
    {
        if(screenImage!=null)
            imageTransform ??= screenImage.GetComponent<RectTransform>();
    }

    private void Awake()
    {
        screenImage.enabled = false;
    }

    public void PlayEffect() => _ = Animate(_destroyCancellation.Token).LogExceptions();

    private async UniTask Animate(CancellationToken cancellationToken)
    {
        ResetTransform();
        var elapsed = 0f;
        while (elapsed < duration)
        {
            var t = elapsed / duration;
            DoRotate(t);
            DoScale(t);
            elapsed += Time.deltaTime;
            await UniTask.Yield();
            cancellationToken.ThrowIfCancellationRequested();
        }

        HideTransform();
        Destroy(_screenshotTexture);
    }

    private void HideTransform()
    {
        imageTransform.localScale = Vector3.zero;
        screenImage.enabled = false;
    }

    private void ResetTransform()
    {
        imageTransform.localScale = Vector3.one;
        imageTransform.rotation = Quaternion.identity;
    }

    private void DoScale(float t)
    {
        var scale = scaleCurve.Evaluate(t);
        imageTransform.localScale = Vector3.one * scale;
    }

    private void DoRotate(float t)
    {
        var speed = Mathf.Lerp(startRotationSpeed, endRotationSpeed, t);
        imageTransform.Rotate(Vector3.forward, speed * Time.deltaTime);
    }

    public void DoScreen()
    {
        _screenshotTexture = ScreenCapture.CaptureScreenshotAsTexture();
        screenImage.texture = _screenshotTexture;
        screenImage.enabled = true;
    }

    private void OnDestroy()
    {
        _destroyCancellation.Cancel();
        _destroyCancellation.Dispose();
    }
}
