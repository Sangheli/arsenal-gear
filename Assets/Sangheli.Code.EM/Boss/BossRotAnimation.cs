using UnityEngine;

public class BossRotAnimation : MonoBehaviour
{
    public Vector3 _zeroLocalPosition;
    private Coroutine _moveCoroutine;

    [Header("Animation Settings")]
    public float moveDownDistance = 1.0f;
    public float moveDownDuration = 0.3f;
    public float returnDuration = 0.3f;

    private void Awake()
    {
        _zeroLocalPosition = transform.localPosition;
    }

    public void ReturnToZero()
    {
        transform.localPosition = _zeroLocalPosition;
    }
}