using UnityEngine;

public interface ICursorInput
{
    void OnInit(Transform currentTransform);
    EInputType GetCursorState(out Vector3 worldPos, Camera camera, Transform currentTransform, float inputMoveSpeed, Vector2 shift, ref bool lastUsedInput, ref Vector2 lastMousePosition);
} 