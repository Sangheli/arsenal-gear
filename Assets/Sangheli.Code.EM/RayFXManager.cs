using System.Collections;
using UnityEngine;

/// <summary>
/// Менеджер эффектов луча.
/// 1) Получает стартовую точку (Vector3)
/// 2) Случайно выбирает конечную XY-точку в пределах мин/макс и фиксирует Z = targetZ
/// 3) Рисует затухающий луч:
///    - Вариант TrailRenderer: движет невидимый объект от старта к цели, оставляя шлейф
///    - Вариант LineRenderer: мгновенная линия, которая плавно исчезает
/// </summary>
public class RayFXManager : MonoBehaviour
{
    [Header("2D-границы (XY)")]
    [Tooltip("Минимальные XY координаты")] public Vector2 minXY = new Vector2(-5, -5);
    [Tooltip("Максимальные XY координаты")] public Vector2 maxXY = new Vector2(5, 5);

    [Header("Фиксированная Z-координата для цели")] 
    public float targetZ = 0f;

    [Header("Префабы для рендера")]
    [Tooltip("TrailRenderer для эффекта шлейфа")] public TrailRenderer trailPrefab;
    [Tooltip("LineRenderer для мгновенной линии")] public LineRenderer linePrefab;

    [Header("Общие настройки")] 
    [Tooltip("Автоуничтожение временных объектов, сек")] public float autoDestroyDelay = 2f;

    /// <summary>
    /// Возвращает случайную конечную точку в пределах мин/макс с фиксированным Z.
    /// </summary>
    public Vector3 GetRandomTarget()
    {
        float x = Random.Range(minXY.x, maxXY.x);
        float y = Random.Range(minXY.y, maxXY.y);
        return new Vector3(x, y, targetZ);
    }

    /// <summary>
    /// Стреляет лучом через TrailRenderer, двигая эмиттер из start в случайную цель.
    /// </summary>
    public void ShootTrail(Vector3 start)
    {
        if (linePrefab == null)
        {
            Debug.LogWarning("Line prefab is not assigned");
            return;
        }

        Vector3 end = GetRandomTarget();

        LineRenderer line = Instantiate(linePrefab);
        line.positionCount = 2;
        line.useWorldSpace = true;
        line.SetPosition(0, start);
        line.SetPosition(1, end);

        // Destroy the line after 0.2 seconds
        Destroy(line.gameObject, 0.2f);
    }

    /// <summary>
    /// Рисует мгновенную линию LineRenderer между start и случайной точкой и плавно гасит её.
    /// </summary>
    public void DrawFadingLine(Vector3 start, float fadeDuration = 0.3f, float startWidth = 0.1f, float endWidth = 0.02f)
    {
        if (linePrefab == null)
        {
            Debug.LogWarning("Line prefab is not assigned");
            return;
        }

        Vector3 end = GetRandomTarget();

        LineRenderer line = Instantiate(linePrefab);
        line.positionCount = 2;
        line.useWorldSpace = true;
        line.SetPosition(0, start);
        line.SetPosition(1, end);

        line.startWidth = startWidth;
        line.endWidth = endWidth;

        var grad = new Gradient();
        grad.SetKeys(
            new[] { new GradientColorKey(Color.white, 0f), new GradientColorKey(Color.white, 1f) },
            new[] { new GradientAlphaKey(1f, 0f), new GradientAlphaKey(1f, 1f) }
        );
        line.colorGradient = grad;

        StartCoroutine(Co_FadeAndDestroy(line, fadeDuration));
    }

    private IEnumerator Co_FadeAndDestroy(LineRenderer line, float duration)
    {
        float t = 0f;
        var grad = line.colorGradient;
        var colorKeys = grad.colorKeys;
        var alphaKeys = grad.alphaKeys;

        while (t < duration)
        {
            t += Time.deltaTime;
            float a = 1f - Mathf.Clamp01(t / duration);

            for (int i = 0; i < alphaKeys.Length; i++)
            {
                alphaKeys[i].alpha = a;
            }
            grad.SetKeys(colorKeys, alphaKeys);
            line.colorGradient = grad;
            yield return null;
        }

        Destroy(line.gameObject, 0.01f);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Vector3 a = new Vector3(minXY.x, minXY.y, targetZ);
        Vector3 b = new Vector3(maxXY.x, minXY.y, targetZ);
        Vector3 c = new Vector3(maxXY.x, maxXY.y, targetZ);
        Vector3 d = new Vector3(minXY.x, maxXY.y, targetZ);
        Gizmos.DrawLine(a, b);
        Gizmos.DrawLine(b, c);
        Gizmos.DrawLine(c, d);
        Gizmos.DrawLine(d, a);
    }
}
