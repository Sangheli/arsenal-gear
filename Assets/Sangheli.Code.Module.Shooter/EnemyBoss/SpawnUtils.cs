using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using System.Collections;

public static class SpawnUtils
{
    public static Vector3 GetScreenCenterWorld(Camera cam)
    {
        var center = cam.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, Mathf.Abs(cam.transform.position.z)));
        center.z = 0;
        return center;
    }

    public static async Task MoveToCenter(Transform obj, Vector3 centerPos, float speed = 10f, CancellationToken cancellationToken = default)
    {
        while (Vector3.Distance(obj.position, centerPos) > 0.1f)
        {
            var direction = (centerPos - obj.position).normalized;
            obj.position += direction * speed * Time.deltaTime;
            await Task.Yield();
            cancellationToken.ThrowIfCancellationRequested();
        }
        obj.position = centerPos;
    }
} 