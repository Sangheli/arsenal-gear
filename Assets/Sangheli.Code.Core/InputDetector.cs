using UnityEngine;

public class InputDetector : MonoBehaviour
{
    private EInputType _inputType;
    public EInputType overrideMobileState;
    
    private void Awake()
    {
        _inputType = Application.isMobilePlatform && Application.platform == RuntimePlatform.WebGLPlayer ? EInputType.Touch :  EInputType.Mouse;
    }
    
    public EInputType GetCurrentInputType
    {
        get
        {
#if UNITY_EDITOR
            if (overrideMobileState != EInputType.None)
                return overrideMobileState;
#endif
            return _inputType;
        }
    }
}