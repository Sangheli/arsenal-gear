using UnityEngine;

public class InputDetector : MonoBehaviour
{
    public EInputType InputType { get; private set; }
    public EInputType overrideMobileState;
    
    public IInputDetectorHandler Handler;

    private void Awake()
    {
        InputType = Application.isMobilePlatform && Application.platform == RuntimePlatform.WebGLPlayer ? EInputType.Touch :  EInputType.Mouse;
        Handler = new InputDetectorYandex(this);
    }
}