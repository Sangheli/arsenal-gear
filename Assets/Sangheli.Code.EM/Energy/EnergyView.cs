using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

public class EnergyView : MonoBehaviour,IReturnable,IResetable
{
    private const float Treshold = 0.1f;
    
    public float flySpeed = 5f;
    [FormerlySerializedAs("flySpeed2")] public float flySpeedFast = 5f;
    public float t1 = 0.2f; // Time to scale up
    public float t2 = 0.3f; // Time to scale down
    public float maxScale = 1.5f; // Maximum scale multiplier
    
    private float maxFlyDistance = 10f; // максимальная дистанция для полёта к игроку

    private bool ScaleDownAnimated { get; set; }
    public bool IsActive { get; private set; }
    public bool CanCollect { get; private set; }
    private bool PlayerFound { get; set; }
    
    private float _radius;
    private Transform _target;
    private Vector3 _originalScale;
    
    public GameObject GameObject => gameObject;
    public GameObject Prefab { get; set; }
    
    public SpriteRenderer viewRenderer;
    public SpriteRenderer shadowRenderer;

    private void Awake()
    {
        _originalScale = transform.localScale;
    }

    private void Update()
    {
        if (!PlayerFound) 
            return;
        
        FlyToPlayer();
        
        if (!PlayerFound) 
            return;
        
        var dist = Vector2.Distance(transform.position, _target.position);
        if (dist > _radius) return;
        TryStartScaleDownAnimation();
    }

    private void FlyToPlayer()
    {
        var dist = Vector2.Distance(transform.position, _target.position);

        // Если игрок слишком далеко — сбрасываем цель
        //if (dist > maxFlyDistance)
        //{
        //    _target = null;
        //    PlayerFound = false;
        //    return;
        //}

        var spd = dist > _radius ? flySpeed : flySpeedFast;
        
        transform.position = Vector2.MoveTowards
        (
            transform.position,
            _target.position,
            spd * Time.deltaTime
        );
    }

    private void TryStartScaleDownAnimation()
    {
        if (ScaleDownAnimated)
            return;

        if (Vector2.Distance(transform.position, _target.position) > Treshold)
            return;

        ScaleDownAnimated = true;
        transform.SetParent(_target);
        G.I.Container.pooledAudioPlayer.PlayWithRandomPitch(G.I.Data.soundContainer.collectCoin);
        StartCoroutine(OnEndScaleAnim());
    }

    private IEnumerator OnEndScaleAnim()
    {
        G.I.EnergyManager.CollectEnergy();
        IsActive = false;
        PlayerFound = false;
        yield return GameAnimation.OnScaleAnim(transform, _originalScale, t1, 1f, maxScale);
        yield return GameAnimation.OnScaleAnim(transform, _originalScale, t2, maxScale, 0f);
        G.I.ObjectPool.Return(this);
    }

    public void Collect(Transform collector, float radius, float maxFlyDistance)
    {
        if (!IsActive) return;
        if (PlayerFound) return;
        
        this.maxFlyDistance = maxFlyDistance;
        CanCollect = false;
        PlayerFound = true;
        _target = collector;
        _radius = radius;
    }

    public void Init()
    {
        CanCollect = true;
        IsActive = true;
        PlayerFound = false;
        ScaleDownAnimated = false;
        _target = null;
    }

    public void ResetItem()
    {
        IsActive = false;
        PlayerFound = false;
        var transform1 = transform;
        transform1.SetParent(null);
        transform1.localScale = _originalScale;
    }

    public void SetSortingOrder(EnergySortingOrderData sortingOrderData)
    {
        viewRenderer.sortingOrder = sortingOrderData.view;
        shadowRenderer.sortingOrder = sortingOrderData.shadow;
    }
}