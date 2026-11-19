using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(ClampPosition))]
public class Weapon2D : MonoBehaviour
{
    public ERayType RayType;
    public EWeaponType Type;
    public float bounceScale = 1.2f;
    public float bounceSpeed = 10f;
    public Vector3 startScale;
    public Transform view;
    public ClampPosition clampPosition;

    private CancellationTokenSource _bounceCancellation;

    public Transform startPosTf;
    public Transform endPosTf;

    private void OnValidate()
    {
        clampPosition ??= GetComponent<ClampPosition>();
    }

    private void Awake()
    {
        startScale = view.transform.localScale;
    }

    public void SetPos(Vector2 pos)
    {
        clampPosition.SetPosition(pos);
    }
    
    public void SetPos(Vector3 pos)
    {
        clampPosition.SetPosition(pos);
    }

    public void Shoot(Vector3 endPos)
    {
        BounceView();
        // ShootRayToTarget(endPosTf.position);
    }

    private void ShootRayToTarget(Vector3 endPos)
    {
        Vector3 startPos = startPosTf.position;
        G.I.rayFXManager.Shoot(startPos, endPos, RayType);
    }

    private void BounceView()
    {
        CancelBounce();
        _bounceCancellation = new CancellationTokenSource();
        _ = BounceAnim(_bounceCancellation.Token);
    }

    private async Task BounceAnim(CancellationToken cancellationToken)
    {
        Vector3 targetScale = startScale * bounceScale;

        // Увеличение
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * bounceSpeed;
            view.localScale = Vector3.Lerp(startScale, targetScale, t);
            await Task.Yield();
            cancellationToken.ThrowIfCancellationRequested();
        }

        // Возврат обратно
        t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * bounceSpeed;
            view.localScale = Vector3.Lerp(targetScale, startScale, t);
            await Task.Yield();
            cancellationToken.ThrowIfCancellationRequested();
        }

        // На всякий случай вернуть точный старт
        view.localScale = startScale;
    }

    private void CancelBounce()
    {
        if (_bounceCancellation == null)
            return;

        _bounceCancellation.Cancel();
        _bounceCancellation.Dispose();
        _bounceCancellation = null;
    }

    private void OnDestroy()
    {
        CancelBounce();
    }
}
