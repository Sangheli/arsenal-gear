using System.Collections;
using UnityEngine;

public class EnemyController : MonoBehaviour, IReturnable, IResetable
{
    public EnemyView view;
    public DamageReciever damageReciever;
    public Vector3 rotationSpeed = new(0, 30, 0);
    private float _speed = 2f;
    public float lifeTime = 3f;
    [Header("Speed Change Settings")]
    public float speedSuper = 20f;
    public float distanceToChangeSpeed = 2000f;

    protected float CurrentHealth;
    private Vector2 _moveDirection;
    private float _originalSpeed;
    private Vector3 _startPosition;
    private bool _isPreSpawned;
    private bool _hasChangedSpeed;
    protected EnemyData CurrentData;
    private Coroutine _lifeTimeCoroutine;
    protected bool IsActive;
    protected bool IsDead;
    private float _targetClipValue;
    private float _currentClipValue;
    private bool _doHpFill;
    private float _fillStartTime;
    public bool manipulateColor = true;
    public bool manipulateBorderColor = false;

    public GameObject GameObject => gameObject;
    public GameObject Prefab { get; set; }
    
    public float duration = 0.2f;


    private void OnValidate()
    {
        damageReciever ??= GetComponentInChildren<DamageReciever>();
        view ??= GetComponentInChildren<EnemyView>();
    }

    private void Awake()
    {
        view ??= GetComponentInChildren<EnemyView>();
        damageReciever.Callback = TakeDamage;
    }

    public void Init(EnemyData data, Vector2 moveDirection, bool isPreSpawned = false)
    {
        IsDead = false;
        SetData(data);
        damageReciever.EnemyData = CurrentData;
        CurrentHealth = CurrentData.Health;
        _moveDirection = moveDirection.normalized;
        _originalSpeed = CurrentData.Speed;
        _speed = _originalSpeed;
        _isPreSpawned = isPreSpawned;
        _hasChangedSpeed = false;
        _startPosition = transform.position;
        if (_isPreSpawned) _speed = speedSuper;
        if (_lifeTimeCoroutine != null) StopCoroutine(_lifeTimeCoroutine);
        _lifeTimeCoroutine = StartCoroutine(LifeTimeCoroutine());
        _doHpFill = false;
        IsActive = true;
        ResetView();
        UpdateColors();
    }

    public void SetData(EnemyData data)
    {
        CurrentData = data;
    }

    public void UpdateColors()
    {
        if (!manipulateColor && manipulateBorderColor)
        {
            view.render.SetBorderColor(CurrentData.borderColor);
            return;
        }
        
        if (!manipulateColor) 
            return;
        
        view.render.SetFullColor(CurrentData.color, CurrentData.borderColor);
        view.deathVfx.SetColor(view.render.OriginalColor);
    }

    public void ResetView()
    {
        _currentClipValue = 0f;
        _targetClipValue = 0f;
        view.SetHitValue(0);
        view.SetFillValue(0);
    }

    private IEnumerator LifeTimeCoroutine()
    {
        yield return new WaitForSeconds(lifeTime);
        if (IsActive)
            G.I.ObjectPool.Return(this);
    }

    private void Update()
    {
        DoFillAction();
        if (!IsActive) return;
        if (_isPreSpawned && !_hasChangedSpeed) CheckAndChangeSpeed();

        DoMove();
    }

    protected virtual void DoMove()
    {
        var deltaTime = Time.deltaTime;
        
        view.render.viewRenderer.transform.Rotate(rotationSpeed * deltaTime);
        view.render.shadowRender.transform.Rotate(rotationSpeed * deltaTime);
        
        if (_hasChangedSpeed && CurrentData.AttractToCursor)
        {
            DoMagnetMove(deltaTime);
        }
        else
        {
            transform.Translate(_moveDirection * (deltaTime * _speed));
        }
    }

    private void DoMagnetMove(float deltaTime)
    {
        // Attraction to cursor logic
        Vector3 cursorPos = G.I.playerCursor.transform.position;
        // Притяжение только в 2D (XY)
        cursorPos.z = transform.position.z;
        Vector3 toCursor = cursorPos - transform.position;
        float dist = toCursor.magnitude;
        if (!(dist > CurrentData.MinAttractDistance))
            return;

        Vector3 moveDir = toCursor.normalized;
        transform.position += moveDir * (CurrentData.AttractionSpeed * deltaTime);
    }

    private void CheckAndChangeSpeed()
    {
        float distanceTraveled = Vector3.Distance(_startPosition, transform.position);
        if (!(distanceTraveled >= distanceToChangeSpeed)) return;
        _speed = _originalSpeed;
        _hasChangedSpeed = true;
    }

    private void DoFillAction()
    {
        if (!_doHpFill) return;
        float elapsedTime = Time.time - _fillStartTime;
        _currentClipValue = Mathf.Lerp(_currentClipValue, _targetClipValue, elapsedTime / duration);
        view.SetFillValue(_currentClipValue);
        if (Mathf.Abs(_currentClipValue - _targetClipValue) < 0.001f)
            _doHpFill = false;
    }

    private void TakeDamage(float amount, bool isCrit)
    {
        if (IsDead) return;
        if (!IsActive) return;
        CurrentHealth -= amount;
        if (CurrentHealth <= 0)
        {
            CurrentHealth = 0;
            IsDead = true;
            Die(isCrit);
        }
        else
        {
            view.DoBounceAnimation(isCrit);
            view.FlashEffect(isCrit);
            if (isCrit) view.CritColorEffect();
        }
        _doHpFill = true;
        _fillStartTime = Time.time;
        _targetClipValue = 1 - CurrentHealth / CurrentData.Health;
    }

    public virtual void Die(bool isCrit)
    {
        if (!IsActive) return;
        StartCoroutine(DeathAndReturnToPool(isCrit));
    }

    private IEnumerator DeathAndReturnToPool(bool isCrit)
    {
        G.I.energySpawner.Spawn(transform.position, CurrentData);
        view.PlayDeathVfx();
        view.DoBounceAnimation(isCrit);
        view.FlashEffect(isCrit);
        IsActive = false;
        yield return new WaitForSeconds(duration);
        yield return view.DeathAnimation();
        G.I.ObjectPool.Return(this);
    }

    public void ResetItem()
    {
        IsActive = false;
        CurrentHealth = 0f;
        _moveDirection = Vector2.zero;
        _doHpFill = false;
        CurrentData = null;
        _isPreSpawned = false;
        _hasChangedSpeed = false;
        _startPosition = Vector3.zero;
        _originalSpeed = 0f;
        _doHpFill = false;
        _currentClipValue = _targetClipValue = 0f;
        _fillStartTime = 0f;
        if (_lifeTimeCoroutine != null) StopCoroutine(_lifeTimeCoroutine);
        _lifeTimeCoroutine = null;
        view.ResetItem();
        transform.SetParent(null);
        transform.localScale = Vector3.one;
    }
} 