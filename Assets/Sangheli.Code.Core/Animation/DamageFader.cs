using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class DamageFader : MonoBehaviour
{
    public float animTime = 0.15f;

    public Color minColor;
    public Color maxColor;

    [Space]
    public Image image;

    [Space]
    public string ParamKey = "_OverlayBlend";
    public string ParamKeyColor = "_OverlayColor";

    public float ParamValueMin;
    public float ParamValueMax = 0.5f;

    public Color blinkColorMax;

    private Material _mat;
    private int _propIndex;
    private int _propIndexColor;
    private CancellationTokenSource _animationCancellation;

    private void Awake()
    {
        _propIndex = Shader.PropertyToID(ParamKey);
        _propIndexColor = Shader.PropertyToID(ParamKeyColor);
        _mat = GetMat(image);

        image.color = minColor;
        _mat.SetColor(_propIndexColor, blinkColorMax);
        _mat.SetFloat(_propIndex, ParamValueMin);
    }

    public static Material GetMat(Image img)
    {
        if (img.material != null)
            img.material = Instantiate(img.material);

        return img.material;
    }

    public void PlayAnim()
    {
        _ = PlayAnimAsync().LogExceptions();
    }

    public async Task PlayAnimAsync()
    {
        CancelAnimation();
        var cancellation = new CancellationTokenSource();
        _animationCancellation = cancellation;
        var token = cancellation.Token;

        try
        {
            await AnimatePhaseAsync(ParamValueMin, ParamValueMax, minColor, maxColor, token);
            await AnimatePhaseAsync(ParamValueMax, ParamValueMin, maxColor, minColor, token);
        }
        finally
        {
            if (_animationCancellation == cancellation)
            {
                _animationCancellation = null;
                cancellation.Dispose();
                if (this != null) Apply(ParamValueMin, minColor);
            }
        }
    }

    private async Task AnimatePhaseAsync(float startBlend, float endBlend, Color startColor, Color endColor, CancellationToken token)
    {
        if (animTime <= 0)
        {
            Apply(endBlend, endColor);
            return;
        }

        var elapsed = 0f;
        while (elapsed < animTime)
        {
            elapsed += Time.deltaTime;
            var progress = Mathf.Clamp01(elapsed / animTime);
            Apply(Mathf.Lerp(startBlend, endBlend, progress), Color.Lerp(startColor, endColor, progress));
            await Task.Yield();
            token.ThrowIfCancellationRequested();
        }

        Apply(endBlend, endColor);
    }

    private void Apply(float blend, Color color)
    {
        _mat.SetFloat(_propIndex, blend);
        image.color = color;
    }

    private void CancelAnimation()
    {
        if (_animationCancellation == null)
            return;

        var cancellation = _animationCancellation;
        _animationCancellation = null;
        cancellation.Cancel();
        cancellation.Dispose();
    }

    private void OnDestroy()
    {
        CancelAnimation();
    }
}
