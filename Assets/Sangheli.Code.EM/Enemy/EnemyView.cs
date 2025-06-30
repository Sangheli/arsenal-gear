using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyView : MonoBehaviour,IReturnable,IResetable
{
    public Transform view;
    public Transform viewShadow;
    public Renderer ViewRenderer;
    public Renderer RamkaRender;
    public Renderer ShadowRender;
    public DamageReciever damageReciever;
    public Vector3 rotationSpeed = new(0, 30, 0);
    
    public float speed = 2f;
    public float lifeTime = 3f;

    public float punchScale = 1.2f;
    public float punchScaleCrit = 1.2f*1.2f;
    public float duration = 0.2f;
    public float maxFlashValue = 0.2f;
    
    // Новые поля для системы изменения скорости
    [Header("Speed Change Settings")]
    public float speedSuper = 20f;
    public float distanceToChangeSpeed = 2000f;
    
    private float _currentHealth;
    
    private Vector2 _moveDirection;
    private Vector3 _originalScale;
    private Vector3 _originalScaleShadow;
    private float _originalSpeed; // Сохраняем изначальную скорость
    private Vector3 _startPosition; // Позиция начала движения
    private bool _isPreSpawned; // Флаг предварительно созданного врага
    private bool _hasChangedSpeed; // Флаг изменения скорости

    private float _targetClipValue;
    private float _currentClipValue;
    private bool _doHpFill;
    private float _fillStartTime; // Время начала анимации заполнения
    
    private MaterialPropertyBlock _propertyBlock;
    private MaterialPropertyBlock _propertyBlockRamka;

    private Coroutine _flashRoutine;
    private static readonly int ClipUvUp = Shader.PropertyToID("_ClipUvUp");
    private static readonly int HitEffectBlend = Shader.PropertyToID("_HitEffectBlend");
    private static readonly int ColorProperty = Shader.PropertyToID("_Color");

    private EnemyData _currentData;
    private Coroutine _lifeTimeCoroutine;
    private bool _isActive;
    private bool _isDead;

    private Color _originalColor;
    private Color _originalRamkaColor;
    private Coroutine _critColorRoutine;

    private Coroutine _deathRoutine;
    public float deathAnimDuration = 0.3f;

    public bool manipulateRamkaColor;
    
    public GameObject GameObject => gameObject;
    public GameObject Prefab { get; set; }

    public DeathVfx deathVfx;

    private void OnValidate()
    {
        damageReciever ??= GetComponentInChildren<DamageReciever>();
    }

    private void Awake()
    {
        _originalScale = view.transform.localScale;
        _originalScaleShadow = viewShadow.transform.localScale;
        damageReciever.Callback = TakeDamage;
        
        _propertyBlock = new MaterialPropertyBlock();
        ViewRenderer.GetPropertyBlock(_propertyBlock);
        
        _propertyBlockRamka = new MaterialPropertyBlock();
        RamkaRender.GetPropertyBlock(_propertyBlockRamka);
        
        _originalColor = ViewRenderer.material.color;
        
        if(manipulateRamkaColor)
            _originalRamkaColor = RamkaRender.material.color;
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
        
        // Инициализация системы изменения скорости
        _isPreSpawned = isPreSpawned;
        _hasChangedSpeed = false;
        _startPosition = transform.position;
        
        // Если враг предварительно создан, устанавливаем увеличенную скорость
        if (_isPreSpawned) speed = speedSuper;

        if (_lifeTimeCoroutine != null) StopCoroutine(_lifeTimeCoroutine);
        _lifeTimeCoroutine = StartCoroutine(LifeTimeCoroutine());
        
        _currentClipValue = 0f;
        _targetClipValue = 0f;
        _doHpFill = false;
        _isActive = true;
        SetHitValue(0);
        SetFillValue(0);
    }

    private IEnumerator LifeTimeCoroutine()
    {
        yield return new WaitForSeconds(lifeTime);
        if (_isActive)
            G.I.ObjectPool.Return(this);
    }

    private void SetHitValue(float value)
    {
        _propertyBlock.SetFloat(HitEffectBlend, value);
        ViewRenderer.SetPropertyBlock(_propertyBlock);
    }

    private void Update()
    {
        DoFillAction();
        
        if (!_isActive) return;
        
        // Проверяем необходимость изменения скорости для предварительно созданных врагов
        if (_isPreSpawned && !_hasChangedSpeed) CheckAndChangeSpeed();


        
        view.Rotate(rotationSpeed * Time.deltaTime);
        viewShadow.Rotate(rotationSpeed * Time.deltaTime);
        transform.Translate(_moveDirection * Time.deltaTime*speed);
    }

    private void CheckAndChangeSpeed()
    {
        // Вычисляем пройденное расстояние
        float distanceTraveled = Vector3.Distance(_startPosition, transform.position);
        
        // Если прошли достаточно далеко, возвращаемся к изначальной скорости
        if (!(distanceTraveled >= distanceToChangeSpeed)) 
            return;
        
        speed = _originalSpeed;
        _hasChangedSpeed = true;
    }

    private void DoFillAction()
    {
        if (!_doHpFill) 
            return;
        
        // Вычисляем прошедшее время с момента начала анимации
        float elapsedTime = Time.time - _fillStartTime;
        
        // Плавно интерполируем значение Clip в течение duration
        _currentClipValue = Mathf.Lerp(_currentClipValue, _targetClipValue, elapsedTime / duration);
        SetFillValue(_currentClipValue);
        
        if (Mathf.Abs(_currentClipValue - _targetClipValue) < 0.001f)
            _doHpFill = false;
    }

    private void TakeDamage(float amount, bool isCrit)
    {
        if(_isDead)
            return;
        
        if (!_isActive) 
            return;
        
        G.I.Container.pooledAudioPlayer.PlayWithRandomPitch(G.I.Data.soundContainer.hit);
        _currentHealth -= amount;
        
        if (_currentHealth <= 0)
        {
            _currentHealth = 0;
            _isDead = true;
            Die(isCrit);
        }
        else
        {
            DoBounceAnimation(isCrit);
            FlashEffect(isCrit);
            if (isCrit)
            {
                if (_critColorRoutine != null) StopCoroutine(_critColorRoutine);
                _critColorRoutine = StartCoroutine(CritColorCoroutine());
            }
        }

        _doHpFill = true;
        _fillStartTime = Time.time; // Записываем время начала анимации
        _targetClipValue = 1-_currentHealth / _currentData.Health;
    }

    private void Die(bool isCrit)
    {
        if (!_isActive) return;
        if (_deathRoutine != null) StopCoroutine(_deathRoutine);
        _deathRoutine = StartCoroutine(DeathAndReturnToPool(isCrit));
    }

    private IEnumerator DeathAndReturnToPool(bool isCrit)
    {
        G.I.energySpawner.Spawn(transform.position, _currentData);
        deathVfx.Play();
        
        var enemyColor = ViewRenderer.material.color;
        // DeathParticleSystem.Instance.PlayDeathEffect(transform.position, enemyColor);
        
        // Добавляем эффекты перед анимацией смерти
        DoBounceAnimation(isCrit);
        FlashEffect(isCrit);
        _isActive = false;
        yield return new WaitForSeconds(duration);
        
        // Анимация сжатия view и viewShadow одновременно
        StartCoroutine(GameAnimation.ShrinkAndDisappear(view, _originalScale, deathAnimDuration));
        yield return StartCoroutine(GameAnimation.ShrinkAndDisappear(viewShadow, _originalScaleShadow, deathAnimDuration));
        G.I.ObjectPool.Return(this);
    }

    private void DoBounceAnimation(bool isCrit = false)
    {
        if (!_isActive || view == null) 
            return;
    
        float scale = isCrit ? punchScaleCrit : punchScale;
        StopCoroutine(nameof(GameAnimation.Bounce));
        StartCoroutine(GameAnimation.Bounce(view.transform ,_originalScale,scale,duration));
        StartCoroutine(GameAnimation.Bounce(viewShadow.transform ,_originalScaleShadow,scale,duration));
    }
    
    private void FlashEffect(bool isCrit = false)
    {
        if (!_isActive) 
            return;
        
        if (_flashRoutine != null)
            StopCoroutine(_flashRoutine);
        
        _flashRoutine = StartCoroutine(GameAnimation.FlashEffect(SetHitValue, isCrit, maxFlashValue, duration));
    }

    private IEnumerator CritColorCoroutine()
    {
        _propertyBlock.SetColor(ColorProperty, Color.red);
        ViewRenderer.SetPropertyBlock(_propertyBlock);
        yield return new WaitForSeconds(duration);
        _propertyBlock.SetColor(ColorProperty, _originalColor);
        ViewRenderer.SetPropertyBlock(_propertyBlock);
    }

    public void ResetItem()
    {
        _isActive = false;
        _currentHealth = 0f;
        _moveDirection = Vector2.zero;
        _doHpFill = false;
        _currentData = null;
        
        // Сброс переменных системы изменения скорости
        _isPreSpawned = false;
        _hasChangedSpeed = false;
        _startPosition = Vector3.zero;
        _originalSpeed = 0f;
        
        _doHpFill = false;
        _currentClipValue = _targetClipValue = 0f;
        _fillStartTime = 0f;
        
        if (_lifeTimeCoroutine != null) StopCoroutine(_lifeTimeCoroutine);
        _lifeTimeCoroutine = null;
        if (_flashRoutine != null) StopCoroutine(_flashRoutine);
        _flashRoutine = null;
        if (_critColorRoutine != null) StopCoroutine(_critColorRoutine);
        _critColorRoutine = null;

        var transform1 = transform;
        var transform2 = view.transform;
        var transform3 = viewShadow.transform;
        
        transform1.SetParent(null);
        transform1.localScale = Vector3.one;
        transform2.localScale = _originalScale;
        transform3.localScale = _originalScaleShadow;
        
        // Сброс вращения
        transform2.localRotation = Quaternion.identity;
        transform3.localRotation = Quaternion.identity;

        SetHitValue(0);
        SetFillValue(_targetClipValue);
        
        _propertyBlock.SetColor(ColorProperty, _originalColor);
        ViewRenderer.SetPropertyBlock(_propertyBlock);

        if (manipulateRamkaColor)
        {
            _propertyBlockRamka.SetColor(ColorProperty, _originalRamkaColor);
            RamkaRender.SetPropertyBlock(_propertyBlockRamka);
        }
    }

    private void SetFillValue(float targetClipValue)
    {
        _propertyBlock.SetFloat(ClipUvUp, targetClipValue);
        ViewRenderer.SetPropertyBlock(_propertyBlock);
    }

    public void SetSortingOrder(int viewOrder, int back)
    {
        ViewRenderer.sortingOrder = back;
        RamkaRender.sortingOrder = viewOrder;
        ShadowRender.sortingOrder = back-1;
        deathVfx.SetOrder(viewOrder);
    }
}