using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputCursor : ICursorInput
{
    private readonly InputSystem_Actions _inputSystemActions;
    public PlayerInputCursor(InputSystem_Actions actions) { _inputSystemActions = actions; }
    
    public void OnInit(Transform currentTransform)
    {
    }
    
    public EInputType GetCursorState(out Vector3 worldPos, Camera camera, Transform currentTransform, float inputMoveSpeed, Vector2 shift, ref bool lastUsedInput, ref Vector2 lastMousePosition)
    {
        var screenPos = Vector3.zero;
        var state = EInputType.None;
        var move = _inputSystemActions.Player.Move.ReadValue<Vector2>();
        var currentMousePosition = Mouse.current != null ? Mouse.current.position.ReadValue() : Vector2.zero;
        var inputType = G.I.inputDetector.GetCurrentInputType;
        
        if (inputType == EInputType.Touch && Touchscreen.current != null && Touchscreen.current.primaryTouch.press.isPressed)
        {
            screenPos = Touchscreen.current.primaryTouch.position.ReadValue();
            state = EInputType.Touch;
        }
        else if (move.sqrMagnitude > 0.01f && inputType == EInputType.Mouse)
        {
            lastUsedInput = true;
            screenPos = camera.WorldToScreenPoint(currentTransform.position) + (Vector3)(move * inputMoveSpeed * Time.deltaTime);
            state = EInputType.Mouse;
        }
        else if (Mouse.current != null && inputType == EInputType.Mouse)
        {
            if (lastUsedInput)
            {
                if (currentMousePosition == lastMousePosition)
                {
                    worldPos = Vector3.zero;
                    return EInputType.None;
                }
            }
            screenPos = currentMousePosition;
            state = EInputType.Mouse;
            lastMousePosition = currentMousePosition;
            lastUsedInput = false;
        }
        screenPos.x = Mathf.Clamp(screenPos.x, 0f, Screen.width);
        screenPos.y = Mathf.Clamp(screenPos.y, 0f, Screen.height);
        screenPos.z =  Mathf.Abs(camera.transform.position.z);
        worldPos = state != EInputType.None ? camera.ScreenToWorldPoint(screenPos) : Vector3.zero;
        worldPos.z = -10f;
        if (state == EInputType.Touch)
            worldPos += (Vector3)shift;
        return state;
    }
} 