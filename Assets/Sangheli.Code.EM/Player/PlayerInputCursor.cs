using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputCursor : ICursorInput
{
    private readonly InputSystem_Actions _inputSystemActions;
    public PlayerInputCursor(InputSystem_Actions actions) { _inputSystemActions = actions; }
    
    public void OnInit(Transform currentTransform)
    {
    }
    
    public CursorState GetCursorState(out Vector3 worldPos, Camera camera, Transform currentTransform, float inputMoveSpeed, Vector2 shift, ref bool lastUsedInput, ref Vector2 lastMousePosition, bool isMobile)
    {
        var screenPos = Vector2.zero;
        var state = CursorState.None;
        var move = _inputSystemActions.Player.Move.ReadValue<Vector2>();
        var currentMousePosition = Mouse.current != null ? Mouse.current.position.ReadValue() : Vector2.zero;
        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.isPressed)
        {
            screenPos = Touchscreen.current.primaryTouch.position.ReadValue();
            state = CursorState.Touch;
        }
        else if (move.sqrMagnitude > 0.01f && !isMobile)
        {
            lastUsedInput = true;
            screenPos = camera.WorldToScreenPoint(currentTransform.position) + (Vector3)(move * inputMoveSpeed * Time.deltaTime);
            state = CursorState.Mouse;
        }
        else if (Mouse.current != null && !isMobile)
        {
            if (lastUsedInput)
            {
                if (currentMousePosition == lastMousePosition)
                {
                    worldPos = Vector3.zero;
                    return CursorState.None;
                }
            }
            screenPos = currentMousePosition;
            state = CursorState.Mouse;
            lastMousePosition = currentMousePosition;
            lastUsedInput = false;
        }
        screenPos.x = Mathf.Clamp(screenPos.x, 0f, Screen.width);
        screenPos.y = Mathf.Clamp(screenPos.y, 0f, Screen.height);
        worldPos = state != CursorState.None ? camera.ScreenToWorldPoint(screenPos) : Vector3.zero;
        worldPos.z = -10f;
        if (state == CursorState.Touch)
            worldPos += (Vector3)shift;
        return state;
    }
} 