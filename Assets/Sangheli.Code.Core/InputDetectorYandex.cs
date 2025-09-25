using YG;

public class InputDetectorYandex : IInputDetectorHandler
{
    private EInputType? _inputType;
    private readonly InputDetector _inputDetector;

    public InputDetectorYandex(InputDetector inputDetector)
    {
        _inputDetector = inputDetector;
    }

    public EInputType GetCurrentInputType()
    {
#if UNITY_EDITOR
        if (_inputDetector.overrideMobileState != EInputType.None)
            return _inputDetector.overrideMobileState;
#endif
        return  _inputType ??= Get();
    }

    private static EInputType Get() =>
        YG2.envir.device switch
        {
            YG2.Device.Desktop => EInputType.Mouse,
            YG2.Device.Mobile => EInputType.Touch,
            YG2.Device.Tablet => EInputType.Touch,
            _ => EInputType.None
        };
}