using UnityEngine;

public class RandomCursorInput : ICursorInput
{
    private Vector3 _direction = Vector3.zero;
    private Vector3 _lastWorldPosition = Vector3.zero;
    private readonly float _speed = 2f;
		
    public void OnInit(Transform currentTransform)
    {
        _direction = Random.insideUnitCircle.normalized;
        _lastWorldPosition = currentTransform.position;
    }

    public EInputType GetCursorState(out Vector3 worldPos, Camera camera, Transform currentTransform, float inputMoveSpeed, Vector2 shift, ref bool lastUsedInput, ref Vector2 lastMousePosition)
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
        return EInputType.Mouse;
    }

    public static bool MoveBounce(Vector3 screenPos, ref Vector3 direction, float paddingPercent)
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
    
    public static bool MoveBounce3D(Vector3 pos, ref Vector3 direction)
    {
        bool bounced = false;
        float minX = G.I.boundBossMin.x;
        float maxX = G.I.boundBossMax.x;
        float minY = G.I.boundBossMin.y;
        float maxY = G.I.boundBossMax.y;

        if (pos.x < minX)
        {
            pos.x = minX;
            direction.x = Mathf.Abs(Random.Range(0.2f, 1f));

            direction = direction.normalized;
            bounced = true;
        }
        else if (pos.x > maxX)
        {
            pos.x = maxX;
            direction.x = -Mathf.Abs(Random.Range(0.2f, 1f));
            
            direction = direction.normalized;
            bounced = true;
        }

        if (pos.y < minY)
        {
            pos.y = minY;
            direction.y = Random.Range(0.1f, 0.5f);
            
            direction = direction.normalized;
            bounced = true;
        }
        else if (pos.y > maxY)
        {
            pos.y = maxY;
            direction.y = Random.Range(-0.1f, -0.5f);
            
            direction = direction.normalized;
            bounced = true;
        }
        
        return bounced;
    }
} 