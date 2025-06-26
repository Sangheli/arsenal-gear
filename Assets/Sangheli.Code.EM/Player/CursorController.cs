using UnityEngine;

public class CursorController : MonoBehaviour
{
    public Camera _Camera;
    public float inputMoveSpeed = 1000f;
    public Vector2 shift = new(-100, 100);
    public CursorView cursorView;
    public ICursorInput cursorInput;
    private bool _lastUsedInput;
    private Vector2 _lastMousePosition;
    private bool _isMobile;
    private bool _canControl;
    private float _nextTime;
    private float _playerFireRate;
    private float _critChance;
    private float _critMultiplier;
    private float _playerDamageMin;
    private int _critTargetsCount;
    private float _currentHarvestScale;
    public float CurrentDamageScale { get; private set; }
    public float collectRadius { get; private set; }
    private readonly Collider2D[] _enemyHits = new Collider2D[256];
    private readonly Collider2D[] _energyHits = new Collider2D[256];
    private int _enemyLayer;
    private int _energyLayer;
    private DataSave _save;
    private bool _isAI;
    private void Awake()
    {
        _Camera = Camera.main;
        _enemyLayer = LayerMask.GetMask("Enemy");
        _energyLayer = LayerMask.GetMask("Energy");
        _isMobile = Application.isMobilePlatform && Application.platform == RuntimePlatform.WebGLPlayer;
        var actions = new InputSystem_Actions();
        actions.Enable();
        cursorInput = new PlayerInputCursor(actions);
        Toggle(false);
    }
    public void Toggle(bool state)
    {
        cursorView.Toggle(state);
        if (state)
        {
            _save = G.I.SaveProvider.Get();
            _canControl = true;
            if(!_isAI) InitCursorParams();
        }
    }
    
    private void InitCursorParams(float damagePercent = 1f, float radiusPercent = 1f, float fireRatePercent = 1f)
    {
        var paramContainer = _save.GetParamContainer();
        CurrentDamageScale = paramContainer.GetValue("damageDiameter") * radiusPercent;
        _currentHarvestScale = paramContainer.GetValue("collectDiameter") * radiusPercent;
        _playerFireRate = paramContainer.GetValue("playerFireRate") * fireRatePercent;
        _critChance = paramContainer.GetValue("critChance");
        _critMultiplier = paramContainer.GetValue("critMultiplier");
        _playerDamageMin = paramContainer.GetValue("playerDamageMin") * damagePercent;
        _critTargetsCount = Mathf.RoundToInt(paramContainer.GetValue("critTargetsCount"));
        collectRadius = (CurrentDamageScale + _currentHarvestScale) / 2;
        cursorView.SetScale(CurrentDamageScale, _currentHarvestScale);
    }
    
    public void SetAIMode(float damagePercent, float radiusPercent, float fireRatePercent)
    {
        _isAI = true;
        _canControl = true;
        _save = G.I.SaveProvider.Get();
        InitCursorParams(damagePercent, radiusPercent, fireRatePercent);
    }
    
    private void Update()
    {
        if (!_canControl) return;
        if (G.I == null) return;
        if (!G.I.roundManager.IsAlive) return;
        if (G.I.roundManager.IsAliveForPlay)
        {
            TryDoDamage();
            CollectEnergy();
        }
        if (cursorInput.GetCursorState(out var worldPos, _Camera, transform, inputMoveSpeed, shift, ref _lastUsedInput, ref _lastMousePosition, _isMobile) == CursorState.None) return;
        transform.position = worldPos;
    }
    private void TryDoDamage()
    {
        if (Time.time <= _nextTime)
            return;
        var interval = 60f / _playerFireRate;
        _nextTime = Time.time + interval;
        cursorView.DoBounceAnimation(CurrentDamageScale);
        var damageResult = DoDamage();
        switch (damageResult.HasDamage)
        {
            case false when !_isAI:
                G.I.Container.pooledAudioPlayer.PlayWithRandomPitch(G.I.Data.soundContainer.hit2);
                break;
            case true:
                G.I.playerCursorHudView.AddDamage(damageResult.TotalDamage);
                break;
        }
        StartCoroutine(GameAnimation.PulseInnerRadius(cursorView.viewRender, cursorView.PropertyBlock, interval));
    }
    private DamageResult DoDamage()
    {
        int count = Physics2D.OverlapCircleNonAlloc(
            transform.position,
            CurrentDamageScale / 2f,
            _enemyHits,
            _enemyLayer);
        int[] indices = new int[count];
        for (int i = 0; i < count; i++) indices[i] = i;
        for (int i = 0; i < count; i++)
        {
            int j = Random.Range(i, count);
            (indices[i], indices[j]) = (indices[j], indices[i]);
        }
        float totalDamage = 0f;
        for (int i = 0; i < count; i++)
        {
            var enemy = _enemyHits[i].GetComponent<DamageReciever>();
            if (enemy == null) continue;
            bool doCrit = i < _critTargetsCount;
            var critResult = doCrit ? CritHelper.RollCrit(_playerDamageMin, _critChance, _critMultiplier) : new CritResult { Damage = _playerDamageMin, IsCrit = false };
            enemy.TakeDamage(critResult.IsCrit ? -critResult.Damage : critResult.Damage);
            totalDamage += enemy.EnemyData.DamagePerHit;
            float reduce = enemy.EnemyData.DamagePerHit;
            float actual = G.I.HpManager.ReduceHpWithReturn(reduce, doBlink: true, ignoreShield: false);
            G.I.playerCursorHudView.AddDamage(actual, 0f);
        }
        return new DamageResult { HasDamage = count > 0, TotalDamage = totalDamage };
    }
    private void CollectEnergy()
    {
        int count = Physics2D.OverlapCircleNonAlloc(
            transform.position,
            collectRadius,
            _energyHits,
            _energyLayer);
        for (int i = 0; i < count; i++)
        {
            var energy = _energyHits[i].GetComponent<EnergyView>();
            if (energy == null)
                continue;
            energy.Collect(transform, CurrentDamageScale / 2);
        }
    }
    public void ToggleControl(bool b) => _canControl = b;
} 