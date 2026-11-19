using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using Debug = System.Diagnostics.Debug;
using Random = UnityEngine.Random;

public class EnemyController : MonoBehaviour, IReturnable, IResetable,IZPosHandler
{
    public Transform shootPoint;
 
    [Space]
    public SineBounceY bounceMove;
    public float unfillTime = 0.1f;
    public EnemyView view;
    public EnemyHPManager HpManager;
    public DamageReciever damageReciever;
    private float _speed = 2f;
    // public float lifeTime = 3f;
    [Header("Speed Change Settings")]
    public float speedSuper = 20f;
    public float distanceToChangeSpeed = 2000f;
    private CancellationTokenSource _selfDestructCancellation;
    private CancellationTokenSource _deathCancellation;

    private Vector2 _moveDirection;
    private float _originalSpeed;
    private Vector3 _startPosition;
    private bool _isPreSpawned;
    private bool _hasChangedSpeed;
    public EnemyData CurrentData { get; protected set; }
    protected bool IsActive;
    protected bool IsDead;
    public bool manipulateColor = true;
    public bool manipulateBorderColor = false;
    public int ZPos { get; set; }
    public Action OnDeathZPosHolder { get; set; }

    public GameObject GameObject => gameObject;
    public GameObject Prefab { get; set; }
    
    public float duration = 0.2f;

    private Color? _colorMain;
    private Color? _borderColor;
    private float _selfDestructTime;

    public int _difficult;
    public float DamageValue { get; private protected set; }
    public bool CanAttack { get; set; }

    private float NextTimeAttack;
    private int _currentLevel;
    
    protected AUnitMove _move;
    private Dictionary<EUnitMove, AUnitMove> _moveDict = new()
    {
        { EUnitMove.Move, new LinearMove() },
        { EUnitMove.Fly, new FlyMove() },
    };
    
    private bool _pingPong;
    
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

    public void Init(EnemyData data, Vector2 moveDirection, int difficult, int currentLevel, bool isPreSpawned = false)
    {
        _pingPong = Random.value > 0.75f;
        IsDead = false;
        _currentLevel = currentLevel;
        SetData(data);
        shootPoint.transform.localPosition = data.shootPoint;
        _difficult = difficult;
        DamageValue = CurrentData.GetDamage(_difficult, currentLevel);
        HpManager.Init(unfillTime, CurrentData.showHPbar,CurrentData.GetHP(difficult));
        bounceMove.Toggle(CurrentData.useBounceAnimation);
        ResetView2();
        _selfDestructTime = CurrentData.GetSelfDestructTime();
        damageReciever.EnemyData = CurrentData;
        _moveDirection = moveDirection.normalized;
        view.render.SetDirection(_moveDirection);
        _originalSpeed = CurrentData.Speed;
        _speed = _originalSpeed;
        _isPreSpawned = isPreSpawned;
        _hasChangedSpeed = CurrentData.spawnData.spawnAtStartHardcore;
        _startPosition = transform.position;
        if (_isPreSpawned &&  !CurrentData.spawnData.spawnAtStartHardcore ) _speed = speedSuper;
        
        CanAttack = false;
        NextTimeAttack = Time.time + CurrentData.GetInitTimeAttack(_currentLevel);
        
        ActivateSelfDestruct();

        IsActive = true;
        ResetView();
        // Always update colors when initializing
        _colorMain = null; 
        _borderColor = null;
        UpdateColors();
        
        _moveDict.TryGetValue(data.UnitMoveType, out _move);
        MoveInit(data, false);
    }

    private void MoveInit(EnemyData data, bool pingPong)
    {
        switch (data.UnitMoveType)
        {
            case EUnitMove.Move:
            case EUnitMove.Fly when !_hasChangedSpeed:
                _move.Init(transform, _speed, G.I.cameraMain, _moveDirection, 0, pingPong,pingPong,  view.render.SetDirection);
                break;
            case EUnitMove.Fly:
                _move.Init(transform, _speed, G.I.cameraMain, Random.insideUnitCircle.normalized, 35, false,false,  view.render.SetDirection);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public void SetData(EnemyData data)
    {
        CurrentData = data;
    }

    public void UpdateColors()
    {
        if (_colorMain == null)
        {
            _colorMain = CurrentData.colorData.GetColor();
            _borderColor = CurrentData.colorData.GetBorderColor(_difficult);
        }

        Debug.Assert(_borderColor != null, nameof(_borderColor) + " != null");
        
        if (!manipulateColor && manipulateBorderColor)
        {
            view.render.SetBorderColor(_borderColor.Value);
            return;
        }
        
        if (!manipulateColor) 
            return;
        
        view.render.SetFullColor(_colorMain.Value, _borderColor.Value);

        var maxColor = CurrentData.colorData.useRandomColor ? _borderColor.Value : CurrentData.colorData.hitVfxColor;
        var minColor = maxColor /1.3f;
        minColor.a = 1;
        view.hitVfx.SetColor(minColor, maxColor);
    }

    public void ResetView()
    {
        view.SetHitValue(0);
        view.SetFillValue(0);
    }

    private void Update()
    {
        if (!IsActive) return;
        if (_isPreSpawned && !_hasChangedSpeed && !CurrentData.spawnData.spawnAtStartHardcore) CheckAndChangeSpeed();

        // Handle fire rate timing
        if (CanAttack)
        {
            if (Time.time > NextTimeAttack)
            {
                DoDamage();
                UpdateTimeAttack();
            }
        }

        DoMove();
    }

    public void UpdateTimeAttack() => NextTimeAttack = Time.time + CurrentData.GetTimeAttack(_currentLevel);

    private void LateUpdate()
    {
        OnLateUpdate();
    }

    protected virtual void OnLateUpdate()
    {
        if(!_hasChangedSpeed) 
            return;
        
        _move.LateUpdate();
        
        if(CurrentData.UnitMoveType != EUnitMove.Fly)
            CheckBounds();
    }

    protected virtual void DoMove()
    {
        var deltaTime = Time.deltaTime;
        
        view.render.viewRenderArr[0].transform.Rotate(CurrentData.rotationSpeed * deltaTime);
        // view.render.shadowRenderArr[0].transform.Rotate(CurrentData.rotationSpeed * deltaTime);
        
        _move.DoMove(deltaTime);
    }

    private void CheckBounds()
    {
        if (transform.position.x < -G.I.BoundX || transform.position.x > G.I.BoundX)
        {
            OnBeforeReturnToPool();
            IsActive = false;
            G.I.ObjectPool.Return(this);
        }
    }

    private void CheckAndChangeSpeed()
    {
        float distanceTraveled = Vector3.Distance(_startPosition, transform.position);
        if (!(distanceTraveled >= distanceToChangeSpeed)) return;
        _speed = _originalSpeed;
        _hasChangedSpeed = true;
        MoveInit(CurrentData, _pingPong);
    }

    private void TakeDamage(float amount, bool isCrit)
    {
        if (IsDead) return;
        if (!IsActive) return;
        HpManager.TakeDamage(amount);
        
        if (HpManager.GetHp() <= 0)
        {
            IsDead = true;
            Die(isCrit, true);
        }
        else
        {
            view.PlayHitVfx();
            view.DoBounceAnimation(isCrit);
            view.FlashEffect(isCrit);
            if (isCrit) view.CritColorEffect();
        }
    }

    public virtual void Die(bool isCrit, bool killedByPlayer)
    {
        if (!IsActive) return;
        
        if (killedByPlayer)
        {
            if(CurrentData.EnemyType == EEnemyType.Robot)
                G.I.levelTargetManager.Data.AddValue(LevelObjectiveType.EnemyCount, 1);
            if(CurrentData.EnemyType == EEnemyType.Factory)
                G.I.levelTargetManager.Data.AddValue(LevelObjectiveType.Factory, 1);
        }
        
        OnBeforeReturnToPool();
        _deathCancellation = new CancellationTokenSource();
        _ = DeathAndReturnToPool(isCrit, _deathCancellation.Token);
    }

    private void OnBeforeReturnToPool()
    {
        // Notify that this enemy is dying
        OnDeathZPosHolder?.Invoke();
        G.I.roundManager.SpawnPositionManager.Release(this);

        // Notify RoundManager that this enemy is destroyed
        G.I.roundManager.OnEnemyDestroyed(this);

        StopSelfDestruct();
    }

    private async Task DeathAndReturnToPool(bool isCrit, CancellationToken cancellationToken)
    {
        G.I.energySpawner.Spawn(transform.position, CurrentData);
        view.PlayDeathVfx();
        view.DoBounceAnimation(isCrit);
        view.FlashEffect(isCrit);
        
        if(CurrentData.playDeathSound)
            G.I.Container.pooledAudioPlayer.PlayWithRandomPitch(G.I.Data.soundContainer.enemyDeathSound);

        if(CurrentData.playDeathSound2)
            G.I.Container.pooledAudioPlayer.PlayWithRandomPitch(G.I.Data.soundContainer.enemyDeathSoundRobot);
        
        IsActive = false;
        await Task.Delay(TimeSpan.FromSeconds(duration), cancellationToken);
        await view.DeathAnimation(cancellationToken);
        G.I.ObjectPool.Return(this);
    }

    private void ResetView2()
    {
        view.render.SetOriginalScale(CurrentData.viewScale, false);
        view.hitVfx.SetScale(CurrentData.hitVfx);
        view.deathVfx.SetScale(CurrentData.deathVfxScale);
        view.render.SetSprites(CurrentData);
    }

    public void ResetItem()
    {
        ResetView2();
        IsActive = false;
        _moveDirection = Vector2.zero;
        CurrentData = null;
        _isPreSpawned = false;
        _hasChangedSpeed = false;
        _startPosition = Vector3.zero;
        _originalSpeed = 0f;
        CancelDeath();
        
        StopSelfDestruct();
        view.ResetItem();
        transform.SetParent(null);
        transform.localScale = Vector3.one;
    }

    private void ActivateSelfDestruct()
    {
        StopSelfDestruct();
        
        if (CurrentData.useSelfDestruct && _selfDestructTime > 0)
        {
            _selfDestructCancellation = new CancellationTokenSource();
            _ = SelfDestructAsync(_selfDestructCancellation.Token);
        }
    }
    
    
    private async Task SelfDestructAsync(CancellationToken cancellationToken)
    {
        await Task.Delay(TimeSpan.FromSeconds(_selfDestructTime), cancellationToken);
        if (!IsActive || IsDead) return;
        IsDead = true;
        Die(false, false);
    }

    private void StopSelfDestruct()
    {
        if (_selfDestructCancellation == null) return;
        _selfDestructCancellation.Cancel();
        _selfDestructCancellation.Dispose();
        _selfDestructCancellation = null;
    }

    private void CancelDeath()
    {
        if (_deathCancellation == null) return;
        _deathCancellation.Cancel();
        _deathCancellation.Dispose();
        _deathCancellation = null;
    }

    private void OnDestroy()
    {
        StopSelfDestruct();
        CancelDeath();
    }
    
    private void DoDamage()
    {
        if(DamageValue<0.001f)
            return;
        
        G.I.HpManager.Reduce(DamageValue, doBlink: true, ignoreShield: false);//shoot to player
        G.I.rayFXManager.ShootToPlayer(shootPoint.position, ERayType.LaserEnemy);
    }
} 