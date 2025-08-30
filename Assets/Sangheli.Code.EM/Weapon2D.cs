using System.Collections;
using UnityEngine;

[RequireComponent(typeof(ClampPosition))]
public class Weapon2D : MonoBehaviour
{
    public EWeaponType Type;
    public float bounceScale = 1.2f;
    public float bounceSpeed = 10f;
    public Vector3 startScale;
    public Transform view;
    public ClampPosition clampPosition;

    private Coroutine bounceRoutine;

    private void OnValidate()
    {
        clampPosition ??= GetComponent<ClampPosition>();
    }

    private void Awake()
    {
        startScale = view.transform.localScale;
    }

    public void SetPos(Vector3 pos)
    {
        clampPosition.SetPosition(pos);
    }

    public void Shoot()
    {
        PlaySound();
        BounceView();
    }

    private void BounceView()
    {
        // если старая анимация ещё идёт – останавливаем её
        if (bounceRoutine != null)
            StopCoroutine(bounceRoutine);

        bounceRoutine = StartCoroutine(BounceAnim());
    }

    private IEnumerator BounceAnim()
    {
        Vector3 targetScale = startScale * bounceScale;

        // Увеличение
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * bounceSpeed;
            view.localScale = Vector3.Lerp(startScale, targetScale, t);
            yield return null;
        }

        // Возврат обратно
        t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * bounceSpeed;
            view.localScale = Vector3.Lerp(targetScale, startScale, t);
            yield return null;
        }

        // На всякий случай вернуть точный старт
        view.localScale = startScale;

        bounceRoutine = null;
    }

    private void PlaySound()
    {
        switch (Type)
        {
            case EWeaponType.None:
                break;
            case EWeaponType.MachineGun:
                G.I.Container.pooledAudioPlayer.PlayWithRandomPitch(G.I.Data.soundContainer.hit);
                break;
            case EWeaponType.Laser:
                G.I.Container.pooledAudioPlayer.PlayWithRandomPitch(G.I.Data.soundContainer.laserShoot);
                break;
        }
    }
}
