using UnityEngine;

public class CursorController : MonoBehaviour
{
    public Camera _Camera;
    public float inputMoveSpeed = 1000f;
    public Vector2 shift = new(-100, 100);
    public CursorView view;
    public ICursorInput CursorInput;
    
    private Vector2 _lastMousePosition;

    private bool _isAI;    
    private bool _lastUsedInput;
    private bool _isMobile;
    private bool _canControl;
    
    private float _nextTime;
    private float _playerFireRate;
    private float _critChance;
    private float _critMultiplier;
    private float _playerDamageMin;
    private float _currentHarvestScale;
    
    public float CurrentDamageScale { get; set; }
    private float CurrentDamageScaleHalf { get; set; }
    private float CollectRadius { get; set; }
    
    private readonly Collider2D[] _enemyHits = new Collider2D[256];
    private readonly Collider2D[] _energyHits = new Collider2D[256];
    
    private int _enemyLayer;
    private int _energyLayer;
    
    private DataSave _save;
    private InputSystem_Actions _actions;
    
    private void Awake()
    {
        _Camera = Camera.main;
        _enemyLayer = LayerMask.GetMask("Enemy");
        _energyLayer = LayerMask.GetMask("Energy");
        _isMobile = Application.isMobilePlatform && Application.platform == RuntimePlatform.WebGLPlayer;
        _actions = new InputSystem_Actions();
        _actions.Enable();
        CursorInput = new PlayerInputCursor(_actions);
        Toggle(false);
    }
    
    public void Toggle(bool state)
    {
        view.Toggle(state);
        if (state)
        {
            _save = G.I.SaveProvider.Get();
            _canControl = true;
            if(!_isAI) InitParams();
            CursorInput.OnInit(transform);
        }
    }
    
    private void InitParams(float damagePercent = 1f, float radiusPercent = 1f, float fireRatePercent = 1f)
    {
        var paramContainer = _save.GetParamContainer();
        CurrentDamageScale = paramContainer.GetValue("damageDiameter") * radiusPercent;
        CurrentDamageScaleHalf = CurrentDamageScale / 2f;
        _currentHarvestScale = paramContainer.GetValue("collectDiameter") * radiusPercent;
        _playerFireRate = paramContainer.GetValue("playerFireRate") * fireRatePercent;
        _critChance = paramContainer.GetValue("critChance")/100f;
        _critMultiplier = paramContainer.GetValue("critMultiplier");
        _playerDamageMin = paramContainer.GetValue("playerDamageMin") * damagePercent;
        CollectRadius = (CurrentDamageScale + _currentHarvestScale) / 2;
        view.SetScale(CurrentDamageScale, _currentHarvestScale);
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
            CollectEnergy();
        }
        if (CursorInput.GetCursorState(out var worldPos, _Camera, transform, inputMoveSpeed, shift, ref _lastUsedInput, ref _lastMousePosition, _isMobile) == CursorState.None) return;
        transform.position = worldPos;
    }
    private void TryDoDamage()
    {
        if (Time.time <= _nextTime)
            return;
        var interval = 60f / _playerFireRate;
        _nextTime = Time.time + interval;
        view.DoBounceAnimation(CurrentDamageScale);
        var damageResult = DoDamage();
        PlaySound(damageResult);
        
        if (!_isAI && damageResult.HasDamage)
        {
            G.I.playerCursor.view.PulseRingColor(Color.red, 0.15f);
            
            G.I.playerCursor.view.hudView.AddDamage
            (
                damageResult,
                CurrentDamageScale
            );
        }

        StartCoroutine(GameAnimation.PulseInnerRadius(view.viewRender, view.PropertyBlock, interval));
    }

    private void PlaySound(DamageResult damageResult)
    {
        if (_isAI) return;
        
        G.I.Container.pooledAudioPlayer.PlayWithRandomPitch(damageResult.HasDamage
            ? G.I.Data.soundContainer.hit
            : G.I.Data.soundContainer.emptyHit);
    }

    private DamageResult DoDamage()
    {
        int count = Physics2D.OverlapCircleNonAlloc(
            transform.position,
            CurrentDamageScaleHalf,
            _enemyHits,
            _enemyLayer);
        
        int[] indices = new int[count];
        
        for (int i = 0; i < count; i++) 
            indices[i] = i;
        
        for (int i = 0; i < count; i++)
        {
            int j = Random.Range(i, count);
            (indices[i], indices[j]) = (indices[j], indices[i]);
        }
        
        float totalDamage = 0f;
        float totalShield = 0f;
        for (int i = 0; i < count; i++)
        {
            var enemy = _enemyHits[i].GetComponent<DamageReciever>();
            if (enemy == null) continue;
            var critResult = CritHelper.RollCrit(_playerDamageMin, _critChance, _critMultiplier);
            enemy.TakeDamage(critResult.IsCrit ? -critResult.Damage : critResult.Damage);
            var finalDamage = G.I.HpManager.Reduce(enemy.EnemyData.DamagePerHit, doBlink: true, ignoreShield: false);
            totalShield += enemy.EnemyData.DamagePerHit - finalDamage;
            totalDamage += finalDamage;
        }
        
        return new DamageResult { HasDamage = count > 0, TotalDamage = totalDamage, TotalShieldDamage = totalShield };
    }
    private void CollectEnergy()
    {
        int count = Physics2D.OverlapCircleNonAlloc(
            transform.position,
            CollectRadius,
            _energyHits,
            _energyLayer);
        
        for (int i = 0; i < count; i++)
        {
            var energy = _energyHits[i].GetComponent<EnergyView>();
            if (energy == null)
                continue;
            energy.Collect(transform, CurrentDamageScaleHalf);
        }
    }
    public void ToggleControl(bool b) => _canControl = b;
    
    private void OnDisable()
    {
        if (_actions == null) 
            return;
        
        _actions.Disable();
        _actions.Dispose();
    }
} 