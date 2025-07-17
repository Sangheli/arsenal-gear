using UnityEngine;

public class RandomCursorInput : ICursorInput
{
    private Vector2 _direction = Vector2.zero;
    private Vector3 _lastWorldPosition = Vector3.zero;
    private readonly float _speed = 2f;
		
    public void OnInit(Transform currentTransform)
    {
        _direction = Random.insideUnitCircle.normalized;
        _lastWorldPosition = currentTransform.position;
    }

    public CursorState GetCursorState(out Vector3 worldPos, Camera camera, Transform currentTransform, float inputMoveSpeed, Vector2 shift, ref bool lastUsedInput, ref Vector2 lastMousePosition, bool isMobile)
    {
        // Двигаем курсор в мировых координатах
        var delta = _direction * _speed * Time.deltaTime;
        worldPos = _lastWorldPosition + new Vector3(delta.x,delta.y);

        var screenPos = camera.WorldToScreenPoint(worldPos);
        var bounced = MoveBounce(screenPos, ref _direction,0);
        worldPos = camera.ScreenToWorldPoint(screenPos);
        _lastWorldPosition = worldPos;
        worldPos.z = -10f;
        
        if (bounced) _direction = _direction.normalized;
        return CursorState.Mouse;
    }

    public static bool MoveBounce(Vector3 screenPos, ref Vector2 direction, float paddingPercent)
    {
        bool bounced = false;
        float paddingX = Screen.width * paddingPercent * 0.01f;
        float paddingY = Screen.height * paddingPercent * 0.01f;
        float minX = paddingX;
        float maxX = Screen.width - paddingX;
        float minY = paddingY;
        float maxY = Screen.height - paddingY;
        
        if (screenPos.x < minX)
        {
            screenPos.x = minX;
            direction.x = Mathf.Abs(Random.Range(0.2f, 1f));
            direction.y = Random.Range(-1f, 1f);
            bounced = true;
        }
        else if (screenPos.x > maxX)
        {
            screenPos.x = maxX;
            direction.x = -Mathf.Abs(Random.Range(0.2f, 1f));
            direction.y = Random.Range(-1f, 1f);
            bounced = true;
        }

        if (screenPos.y < minY)
        {
            screenPos.y = minY;
            direction.y = Mathf.Abs(Random.Range(0.2f, 1f));
            direction.x = Random.Range(-1f, 1f);
            bounced = true;
        }
        else if (screenPos.y > maxY)
        {
            screenPos.y = maxY;
            direction.y = -Mathf.Abs(Random.Range(0.2f, 1f));
            direction.x = Random.Range(-1f, 1f);
            bounced = true;
        }
        
        return bounced;
    }
} 