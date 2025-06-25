using UnityEngine;

public class RandomCursorInput : ICursorInput
{
    private Vector2 _direction = Vector2.zero;
    private float _speed = 500f;
    private float _changeDirCooldown = 0.5f;
    private float _lastChangeTime = 0f;
    private Vector2 _lastPosition = Vector2.zero;
    private bool _initialized = false;

    public CursorState GetCursorState(out Vector3 worldPos, Camera camera, Transform currentTransform, float inputMoveSpeed, Vector2 shift, ref bool lastUsedInput, ref Vector2 lastMousePosition, bool isMobile)
    {
        // Инициализация направления и позиции
        if (!_initialized)
        {
            _direction = Random.insideUnitCircle.normalized;
            _lastPosition = camera.WorldToScreenPoint(currentTransform.position);
            _initialized = true;
        }

        // Случайно меняем направление иногда
        if (Time.time - _lastChangeTime > _changeDirCooldown)
        {
            if (Random.value < 0.2f) // 20% шанс сменить направление
            {
                _direction = Random.insideUnitCircle.normalized;
            }
            _lastChangeTime = Time.time;
        }

        // Двигаем курсор
        Vector2 screenPos = _lastPosition + _direction * _speed * Time.deltaTime;

        // Проверяем на выход за границы экрана
        bool bounced = false;
        if (screenPos.x < 0f)
        {
            screenPos.x = 0f;
            _direction.x = Mathf.Abs(Random.Range(0.2f, 1f));
            _direction.y = Random.Range(-1f, 1f);
            bounced = true;
        }
        else if (screenPos.x > Screen.width)
        {
            screenPos.x = Screen.width;
            _direction.x = -Mathf.Abs(Random.Range(0.2f, 1f));
            _direction.y = Random.Range(-1f, 1f);
            bounced = true;
        }
        if (screenPos.y < 0f)
        {
            screenPos.y = 0f;
            _direction.y = Mathf.Abs(Random.Range(0.2f, 1f));
            _direction.x = Random.Range(-1f, 1f);
            bounced = true;
        }
        else if (screenPos.y > Screen.height)
        {
            screenPos.y = Screen.height;
            _direction.y = -Mathf.Abs(Random.Range(0.2f, 1f));
            _direction.x = Random.Range(-1f, 1f);
            bounced = true;
        }
        
        if (bounced) _direction = _direction.normalized;

        _lastPosition = screenPos;
        worldPos = camera.ScreenToWorldPoint(screenPos);
        worldPos.z = -10f;
        return CursorState.Mouse;
    }
} 