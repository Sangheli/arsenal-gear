using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CursorController : MonoBehaviour
{
    public Transform coinCollectorTf;
    public float inputMoveSpeed = 1000f;
    public Vector2 shift = new(-100, 100);
    public CursorView view;
    public ICursorInput CursorInput;
    
    private Vector2 _lastMousePosition;

    private bool _isAI;    
    private bool _lastUsedInput;
    private bool _canControl;
    
    private float _nextTime;
    private float _playerFireRate;
    private float _critChance;
    private float _critMultiplier;
    private ParamInfo _damageParam;
    private float _damagePercent;
    
    public float baseInner = 0.234f;
    private float _baseInnerCurrent = 0.234f;

    private float _currentHarvestScale;
    
    private readonly CursorDamageHelper3D _damageHelper = new();
    private readonly CursorCollectorHelper3D _collectorHelper = new();

    public Collider2D collider2D;
    
    public float CurrentDamageScale { get; set; }
    private float CurrentDamageScaleHalf { get; set; }
    private float CollectRadius { get; set; }
    
    private DataSave _save;
    private InputSystem_Actions _actions;
    
    private float _damageValueSum;
    private bool _autoAttack = true;
    public float radiusGizmos = 1f;
    public Color colorGizmos = Color.green;
    
    private void Awake()
    {
        _damageHelper.Init();
        _collectorHelper.Init();
        _actions = new InputSystem_Actions();
        _actions.Enable();
        CursorInput = new PlayerInputCursor(_actions);
        Toggle(false);
    }
    
    public void Toggle(bool state)
    {
        view.Toggle(state);
        CursorController.ToggleSystemCursor(!state);
        if (state)
        {
            _save = G.I.SaveProvider.Get();
            _canControl = true;
            if(!_isAI) InitParams();
            CursorInput.OnInit(transform);
        }
    }

    public static void ToggleSystemCursor(bool state)
    {
#if !UNITY_EDITOR && !UNITY_ANDROID && !UNITY_IOS
        Cursor.visible = state;
#endif        
    }
    
    private void InitParams(float damagePercent = 1f, float radiusPercent = 1f, float fireRatePercent = 1f)
    {
        transform.position = new Vector3(0, 0, -10);
        _damageHelper.Reset();
        var paramContainer = _save.GetParamContainer();
        CurrentDamageScale = paramContainer.GetValue("damageDiameter") * radiusPercent;
        CurrentDamageScaleHalf = CurrentDamageScale / 2f;
        _currentHarvestScale = paramContainer.GetValue("collectDiameter") * radiusPercent;
        _playerFireRate = FireRateManager.GetPlayerFireRate() * fireRatePercent;
        _critChance = paramContainer.GetValue("critChance")/100f;
        _critMultiplier = paramContainer.GetValue("critMultiplier");
        _damagePercent = damagePercent;
        _damageParam = paramContainer.GetCachedParamInfo("playerDamageMin");
        var superMult = GetSuperMult();
        _damageValueSum = DPSManager.keys.Sum(x => paramContainer.GetValue(x))*superMult;
        CollectRadius = (_currentHarvestScale) / 2;
        CollectRadius = CurrentDamageScaleHalf;
        view.isPulsing = false;
        view.ApplyVisual();
        view.SetScale(CurrentDamageScale, _currentHarvestScale);

        _baseInnerCurrent = baseInner / CurrentDamageScale;
        
        view.PropertyBlock.SetFloat(GameAnimation.PropInnerRadius, _baseInnerCurrent);
        view.viewRender.SetPropertyBlock(view.PropertyBlock);
        
        _autoAttack = G.I.currentLevel.playerAutoAttack;

        if (_autoAttack)
        {
            UpdateNextTime();
            ShowPulseAnimation();
        }
    }

    private int GetSuperMult()
    {
        var paramContainer = _save.GetParamContainer();
        var mCount = (int) paramContainer.GetValue("u_machineGunCount");
        var lCount = (int) paramContainer.GetValue("u_LaserCount");
        return mCount + lCount;
    }
    
    public void SetAIMode(float damagePercent, float radiusPercent, float fireRatePercent)
    {
        _isAI = true;
        _canControl = true;
        _save = G.I.SaveProvider.Get();
        InitParams(damagePercent, radiusPercent, fireRatePercent);
    }
    
    private void Update()
    {
        if (!_canControl) return;
        if (G.I == null) return;
        if (!G.I.CanPlay) return;
        if (!G.I.roundManager.IsAlive) return;
        if (G.I.roundManager.IsAliveForPlay)
        {
            TryDoDamage();
            _collectorHelper.CollectEnergy(coinCollectorTf, CollectRadius, CurrentDamageScaleHalf);
        }
        if (CursorInput.GetCursorState(out var worldPos, G.I.cameraMain, transform, inputMoveSpeed, shift, ref _lastUsedInput, ref _lastMousePosition) == EInputType.None) return;
        transform.position = worldPos;
    }
    private void TryDoDamage()
    {
        if(!_autoAttack)
            return;
        
        if (Time.time <= _nextTime)
            return;
        
        // view.isPulsing = false;
        // view.ApplyVisual();
        
        UpdateNextTime();
        // ShowPulseAnimation();
        
        // var damageResult = _damageHelper.DoDamage
        // (
        //     transform, _isAI, 
        //     _damageParam, _damageValueSum, _damagePercent,
        //     _critChance, _critMultiplier,
        //     CurrentDamageScaleHalf
        // );
        var damageResult = _damageHelper.DoDamage(
            G.I.cameraMain,
            isAI: false,
            damageParam: _damageParam,
            damageValue: _damageValueSum,
            damagePercent: _damagePercent,
            critChance: _critChance,
            critMultiplier: _critMultiplier,
            currentDamageScaleHalf: CurrentDamageScaleHalf
        );
        
        if(_isAI) return;
        
        // if (damageResult.HasRecivedDamage) 
        //     G.I.playerCursor.view.PulseRingColor(Color.red, 0.15f);

        if (damageResult.PlayerDoDamage)
        {
            G.I.weaponManager.Shoot(damageResult.targetToShoot);
            view.DoBounceAnimation(CurrentDamageScale);
        }
        
        // PlaySound(damageResult);
    }

    private void PlaySound(DamageResult damageResult)
    {
        G.I.Container.pooledAudioPlayer.PlayWithRandomPitch(damageResult.PlayerDoDamage
            ? G.I.Data.soundContainer.hit
            : G.I.Data.soundContainer.emptyHit);
        
    }

    private void ShowPulseAnimation()
    {
        var interval = 60f / _playerFireRate;
        StartCoroutine(GameAnimation.PulseInnerRadius(view.viewRender, view.PropertyBlock, interval, _baseInnerCurrent));
    }

    private void UpdateNextTime()
    {
        var interval = 60f / _playerFireRate;
        _nextTime = Time.time + interval;
    }

    public void ToggleControl(bool b, bool showDeath)
    {
        _canControl = b;
        if (!b && showDeath) view.PlayDeathAnimation();
    }

    private void OnDisable()
    {
        if (_actions == null) 
            return;
        
        _actions.Disable();
        _actions.Dispose();
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = colorGizmos;
        var position = transform.position;
        
        Gizmos.DrawWireSphere(position, radiusGizmos/100f); // каркас сферы
        Gizmos.DrawSphere(position, radiusGizmos /100f); // маленькая заливка в центре

        _damageHelper?.DrawGizmos();
    }
} 