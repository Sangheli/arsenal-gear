using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HalfBarView : MonoBehaviour
{
    public Image hpImage;
    public Image hpImageWhiteBack;
    public Image hpImageBack;
    
    private Color _startColor;
    private Coroutine _blinkCoroutine;
    private Coroutine _bounceCoroutine;
    
    public float UnFillTime { get; set; }

    private const float ClipplingLeftFill = 0;
    private float _clipplingRightFill;
    
    private float _clipplingLeftWhite;
    private float _clipplingRightWhite;
    
    private float _clipplingLeftBack;
    private readonly float _clipplingRightBack = 0;

    private float _oldFillValue;
    
    private Material _hpImageMat;
    private Material _hpImageWhiteBackMat;
    private Material _hpImageBackMat;

    private static readonly int ClipUvLeft = Shader.PropertyToID("_ClipUvLeft");
    private static readonly int ClipUvRight = Shader.PropertyToID("_ClipUvRight");

    private void Awake()
    {
        _hpImageMat = GetMat(hpImage);
        _hpImageWhiteBackMat = GetMat(hpImageWhiteBack);
        _hpImageBackMat = GetMat(hpImageBack);
    }
    
    private static Material GetMat(Image img)
    {
        if (img.material != null)
            img.material = Instantiate(img.material);

        return img.material;
    }
    
    public void Show(float fillValue, bool doBlink)
    {
        if (!gameObject.activeSelf)
            return;

        if (doBlink)
        {
            Recalc(fillValue, _oldFillValue);
            UpdateView();
            
            if (_blinkCoroutine != null) StopCoroutine(_blinkCoroutine);
            _blinkCoroutine = StartCoroutine(AnimateFill(fillValue, _oldFillValue, UnFillTime));
        }
        else
        {
            Recalc(fillValue, fillValue);
            UpdateView();
            
            _oldFillValue = fillValue;
        }
    }

    private void Recalc(float fillValue, float oldFillValue)
    {
        _clipplingRightFill = 1 - fillValue;
        
        _clipplingLeftWhite = fillValue;
        _clipplingRightWhite = 1 - oldFillValue;
        
        _clipplingLeftBack = oldFillValue;
    }

    private void UpdateView()
    {
        _hpImageMat.SetFloat(ClipUvLeft, ClipplingLeftFill);
        _hpImageMat.SetFloat(ClipUvRight, _clipplingRightFill);
        
        _hpImageWhiteBackMat.SetFloat(ClipUvLeft, _clipplingLeftWhite);
        _hpImageWhiteBackMat.SetFloat(ClipUvRight, _clipplingRightWhite);
        
        _hpImageBackMat.SetFloat(ClipUvLeft, _clipplingLeftBack);
        _hpImageBackMat.SetFloat(ClipUvRight, _clipplingRightBack);
    }

    private IEnumerator AnimateFill(float fillValue, float oldValue, float blinkTime)
    {
        float elapsed = 0f;
        while (elapsed < blinkTime)
        {
            elapsed += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsed / blinkTime);
            
            var newValue = Mathf.Lerp(oldValue, fillValue, progress);
            _hpImageWhiteBackMat.SetFloat(ClipUvRight, 1-newValue);
            _hpImageBackMat.SetFloat(ClipUvLeft, newValue);
            
            yield return null;
        }
        
        _hpImageWhiteBackMat.SetFloat(ClipUvRight, 1-fillValue);
        _hpImageBackMat.SetFloat(ClipUvLeft, fillValue);
        _oldFillValue = fillValue;
    }
}
