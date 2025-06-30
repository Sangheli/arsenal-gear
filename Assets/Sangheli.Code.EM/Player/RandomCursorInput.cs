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

        // Проверяем на выход за границы экрана
        Vector3 screenPos = camera.WorldToScreenPoint(worldPos);
        bool bounced = false;
        if (screenPos.x < 0f)
        {
            screenPos.x = 0f;
            worldPos = camera.ScreenToWorldPoint(screenPos);
            _direction.x = Mathf.Abs(Random.Range(0.2f, 1f));
            _direction.y = Random.Range(-1f, 1f);
            bounced = true;
        }
        else if (screenPos.x > Screen.width)
        {
            screenPos.x = Screen.width;
            worldPos = camera.ScreenToWorldPoint(screenPos);
            _direction.x = -Mathf.Abs(Random.Range(0.2f, 1f));
            _direction.y = Random.Range(-1f, 1f);
            bounced = true;
        }
        if (screenPos.y < 0f)
        {
            screenPos.y = 0f;
            worldPos = camera.ScreenToWorldPoint(screenPos);
            _direction.y = Mathf.Abs(Random.Range(0.2f, 1f));
            _direction.x = Random.Range(-1f, 1f);
            bounced = true;
        }
        else if (screenPos.y > Screen.height)
        {
            screenPos.y = Screen.height;
            worldPos = camera.ScreenToWorldPoint(screenPos);
            _direction.y = -Mathf.Abs(Random.Range(0.2f, 1f));
            _direction.x = Random.Range(-1f, 1f);
            bounced = true;
        }
        
        if (bounced) _direction = _direction.normalized;

        _lastWorldPosition = worldPos;
        worldPos.z = -10f;
        return CursorState.Mouse;
    }
} 