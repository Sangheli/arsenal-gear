using UnityEngine;
using UnityEngine.InputSystem;

public class CursorDamageHelper3D
{
    private int _enemyLayer;
    private readonly RaycastHit[] _enemyHits = new RaycastHit[256];

    // Данные для гизмосов
    private Vector3 LastOrigin { get; set; }
    private Vector3 LastDir { get; set; }
    private float LastRadius { get; set; }
    private float LastDistance { get; set; }
    private Vector3[] LastHitPoints { get; } = new Vector3[256];
    private int LastHitCount { get; set; }

    public void Init()
    {
        _enemyLayer = LayerMask.GetMask("Enemy");
    }

    public DamageResult DoDamage(Camera camera, bool isAI,
        ParamInfo damageParam, float damageValue, float damagePercent,
        float critChance, float critMultiplier, float currentDamageScaleHalf)
    {
        var ray = camera.ScreenPointToRay(GetInputPosition());
        var distance = 1000f;

        // сохраняем данные для гизмосов
        LastOrigin = ray.origin;
        LastDir = ray.direction;
        LastRadius = currentDamageScaleHalf;
        LastDistance = distance;

        const int maxTargets = 1; // максимальное количество целей, которые нужно поразить
        var count = Physics.SphereCastNonAlloc(
            ray,
            currentDamageScaleHalf,
            _enemyHits,
            distance,
            _enemyLayer
        );

        // сортируем по расстоянию к камере
        var hitCount = Mathf.Min(count, maxTargets);

        // простой in-place выбор ближайших N (без аллокаций)
        for (var i = 0; i < hitCount; i++)
        {
            var closestIndex = i;
            for (var j = i + 1; j < count; j++)
            {
                if (_enemyHits[j].distance < _enemyHits[closestIndex].distance)
                    closestIndex = j;
            }

            // меняем местами
            if (closestIndex != i)
                (_enemyHits[i], _enemyHits[closestIndex]) = (_enemyHits[closestIndex], _enemyHits[i]);
        }

        // теперь первые hitCount попаданий — ближайшие
        LastHitCount = hitCount;
        for (int i = 0; i < hitCount; i++) 
            LastHitPoints[i] = _enemyHits[i].point;

        var totalDamage = 0f;
        var totalShield = 0f;

        Transform targetToShoot = null;
        
        for (int i = 0; i < hitCount; i++)
        {
            var enemy = _enemyHits[i].collider.GetComponent<DamageReciever>();
            if (enemy == null) continue;

            if(enemy.EnemyData.cantDie) continue;
            
            var critResult = CritHelper.RollCrit(
                damageParam.GetValueRandomFromValue(damageValue) * damagePercent,
                critChance, critMultiplier
            );

            enemy.TakeDamage(critResult);
            targetToShoot = enemy.transform;
            
            if (isAI) continue;

            // пример "ответки"
            // var finalDamage = G.I.HpManager.Reduce(enemy.EnemyData.DamagePerHit, doBlink: true, ignoreShield: false);
            // totalShield += enemy.EnemyData.DamagePerHit - finalDamage;
            // totalDamage += finalDamage;
        }

        return new DamageResult
        {
            PlayerDoDamage = hitCount > 0,
            TotalDamage = totalDamage,
            TotalShieldDamage = totalShield,
            targetToShoot = targetToShoot == null ? Vector3.zero : targetToShoot.position,
        };
    }

    private static Vector2 GetInputPosition() =>
        Touchscreen.current != null && Touchscreen.current.primaryTouch.press.isPressed
            ? Touchscreen.current.primaryTouch.position.ReadValue()
            : Mouse.current.position.ReadValue();

    // Метод для гизмосов
    public void DrawGizmos()
    {
        Gizmos.color = Color.red;

        // основная линия рейкаста
        Gizmos.DrawLine(LastOrigin, LastOrigin + LastDir * LastDistance);

        // диски начала и конца
        DrawWireCircle(LastOrigin, LastDir, LastRadius);
        DrawWireCircle(LastOrigin + LastDir * LastDistance, LastDir, LastRadius);

        // соединительные линии цилиндра
        Vector3 right = Vector3.Cross(LastDir, Vector3.up).normalized * LastRadius;
        Gizmos.DrawLine(LastOrigin + right, LastOrigin + LastDir * LastDistance + right);
        Gizmos.DrawLine(LastOrigin - right, LastOrigin + LastDir * LastDistance - right);

        Vector3 up = Vector3.Cross(LastDir, right).normalized * LastRadius;
        Gizmos.DrawLine(LastOrigin + up, LastOrigin + LastDir * LastDistance + up);
        Gizmos.DrawLine(LastOrigin - up, LastOrigin + LastDir * LastDistance - up);

        // точки попаданий
        Gizmos.color = Color.green;
        for (int i = 0; i < LastHitCount; i++)
        {
            Gizmos.DrawSphere(LastHitPoints[i], 0.1f);
        }
    }

    // вспомогательная функция для круга
    private void DrawWireCircle(Vector3 center, Vector3 normal, float radius, int segments = 24)
    {
        Vector3 up = Vector3.Cross(normal, Vector3.right);
        if (up == Vector3.zero) up = Vector3.Cross(normal, Vector3.up);
        up.Normalize();
        Vector3 right = Vector3.Cross(normal, up).normalized;

        Vector3 prevPoint = center + right * radius;
        float angleStep = 360f / segments;

        for (int i = 1; i <= segments; i++)
        {
            float angle = angleStep * i * Mathf.Deg2Rad;
            Vector3 nextPoint = center + (right * Mathf.Cos(angle) + up * Mathf.Sin(angle)) * radius;
            Gizmos.DrawLine(prevPoint, nextPoint);
            prevPoint = nextPoint;
        }
    }
}