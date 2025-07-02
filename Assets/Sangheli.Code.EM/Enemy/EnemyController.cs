using System.Collections;
using UnityEngine;

public class EnemyController : MonoBehaviour, IReturnable, IResetable
{
    public EnemyView view;
    public DamageReciever damageReciever;
    public Vector3 rotationSpeed = new(0, 30, 0);
    private float speed = 2f;
    public float lifeTime = 3f;
    [Header("Speed Change Settings")]
    public float speedSuper = 20f;
    public float distanceToChangeSpeed = 2000f;

    protected float _currentHealth;
    private Vector2 _moveDirection;
    private float _originalSpeed;
    private Vector3 _startPosition;
    private bool _isPreSpawned;
    private bool _hasChangedSpeed;
    protected EnemyData _currentData;
    private Coroutine _lifeTimeCoroutine;
    protected bool _isActive;
    protected bool _isDead;
    private float _targetClipValue;
    private float _currentClipValue;
    private bool _doHpFill;
    private float _fillStartTime;

    public GameObject GameObject => gameObject;
    public GameObject Prefab { get; set; }

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
        _isDead = false;
        _currentData = data;
        damageReciever.EnemyData = _currentData;
        _currentHealth = _currentData.Health;
        _moveDirection = moveDirection.normalized;
        _originalSpeed = _currentData.Speed;
        speed = _originalSpeed;
        _isPreSpawned = isPreSpawned;
        _hasChangedSpeed = false;
        _startPosition = transform.position;
        if (_isPreSpawned) speed = speedSuper;
        if (_lifeTimeCoroutine != null) StopCoroutine(_lifeTimeCoroutine);
        _lifeTimeCoroutine = StartCoroutine(LifeTimeCoroutine());
        _currentClipValue = 0f;
        _targetClipValue = 0f;
        _doHpFill = false;
        _isActive = true;
        view.SetHitValue(0);
        view.SetFillValue(0);
    }

    private IEnumerator LifeTimeCoroutine()
    {
        yield return new WaitForSeconds(lifeTime);
        if (_isActive)
            G.I.ObjectPool.Return(this);
    }

    private void Update()
    {
        DoFillAction();
        if (!_isActive) return;
        if (_isPreSpawned && !_hasChangedSpeed) CheckAndChangeSpeed();

        var deltaTime = Time.deltaTime;
        
        view.view.Rotate(rotationSpeed * deltaTime);
        view.viewShadow.Rotate(rotationSpeed * deltaTime);
        transform.Translate(_moveDirection * (deltaTime * speed));
    }

    private void CheckAndChangeSpeed()
    {
        float distanceTraveled = Vector3.Distance(_startPosition, transform.position);
        if (!(distanceTraveled >= distanceToChangeSpeed)) return;
        speed = _originalSpeed;
        _hasChangedSpeed = true;
    }

    private void DoFillAction()
    {
        if (!_doHpFill) return;
        float elapsedTime = Time.time - _fillStartTime;
        _currentClipValue = Mathf.Lerp(_currentClipValue, _targetClipValue, elapsedTime / view.duration);
        view.SetFillValue(_currentClipValue);
        if (Mathf.Abs(_currentClipValue - _targetClipValue) < 0.001f)
            _doHpFill = false;
    }

    private void TakeDamage(float amount, bool isCrit)
    {
        if (_isDead) return;
        if (!_isActive) return;
        _currentHealth -= amount;
        if (_currentHealth <= 0)
        {
            _currentHealth = 0;
            _isDead = true;
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
        _targetClipValue = 1 - _currentHealth / _currentData.Health;
    }

    public virtual void Die(bool isCrit)
    {
        if (!_isActive) return;
        StartCoroutine(DeathAndReturnToPool(isCrit));
    }

    private IEnumerator DeathAndReturnToPool(bool isCrit)
    {
        G.I.energySpawner.Spawn(transform.position, _currentData);
        view.PlayDeathVfx();
        view.DoBounceAnimation(isCrit);
        view.FlashEffect(isCrit);
        _isActive = false;
        yield return new WaitForSeconds(view.duration);
        StartCoroutine(GameAnimation.ShrinkAndDisappear(view.view, view.view.transform.localScale, view.deathAnimDuration));
        yield return StartCoroutine(GameAnimation.ShrinkAndDisappear(view.viewShadow, view.viewShadow.transform.localScale, view.deathAnimDuration));
        G.I.ObjectPool.Return(this);
    }

    public void ResetItem()
    {
        _isActive = false;
        _currentHealth = 0f;
        _moveDirection = Vector2.zero;
        _doHpFill = false;
        _currentData = null;
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