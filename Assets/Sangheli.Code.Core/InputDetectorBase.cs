internal class InputDetectorBase : IInputDetectorHandler
{
    private InputDetector _inputDetector;

    public InputDetectorBase(InputDetector inputDetector)
    {
        _inputDetector = inputDetector;
    }
    
    public EInputType GetCurrentInputType()
    {
#if UNITY_EDITOR
            if (_inputDetector.overrideMobileState != EInputType.None)
                return _inputDetector.overrideMobileState;
#endif
            return _inputDetector.InputType;
    }
}