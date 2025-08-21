using UnityEngine;

public class HalfBarView : MonoBehaviour
{
    public ARunningBar runningBarImage;
    public string lowerBorder = "_ClipUvLeft";
    public string upperBorder = "_ClipUvRight";

    private RunningBarValue _runningBarValue;
    private float _oldFillValue;
    
    public float UnFillTime { get; set; }

    // Animation variables
    private bool _isAnimating;
    
    private float _animationStartTime;
    private float _animationStartValue;
    private float _animationTargetValue;

    private void Awake()
    {
        var clipUvLeft = Shader.PropertyToID(lowerBorder);
        var clipUvRight = Shader.PropertyToID(upperBorder);
        
        _runningBarValue = new RunningBarValue(clipUvLeft, clipUvRight);
        runningBarImage.Set(_runningBarValue);
    }

    public void Show(float fillValue, bool doBlink, bool useStartValue)
    {
        if (useStartValue)
            _oldFillValue = fillValue;

        if (!gameObject.activeSelf)
            return;

        if (doBlink && fillValue < _oldFillValue)
        {
            _runningBarValue.Recalc(fillValue, _oldFillValue);
            runningBarImage.UpdateView();
            
            _isAnimating = true;
            _animationStartTime = Time.time;
            _animationStartValue = _oldFillValue;
            _animationTargetValue = fillValue;
        }
        else
        {
            _runningBarValue.Recalc(fillValue, _oldFillValue);
            runningBarImage.UpdateView();
            
            _oldFillValue = fillValue;
            _animationTargetValue = fillValue;
        }
    }

    private void Update()
    {
        if (!_isAnimating) 
            return;
        
        var elapsed = Time.time - _animationStartTime;
        var progress = Mathf.Clamp01(elapsed / UnFillTime);
        var newValue = Mathf.Lerp(_animationStartValue, _animationTargetValue, progress);

        var finished = progress >= 1.0f;
        runningBarImage.SetCurrentValue(finished? _animationTargetValue:  newValue);
        _oldFillValue = newValue;

        if (!finished) 
            return;
        
        _isAnimating = false;
        _oldFillValue = _animationTargetValue;
    }
}
