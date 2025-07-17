using UnityEngine;
using DG.Tweening;

public class BossController : EnemyController
{
    public BossRotAnimation rotAnimation;
    private System.Action _onBossDeath;
    private Vector2 _direction = Vector2.zero;
    public float speedBoss = 2f;
    private Camera _mainCamera;
    public float paddingMove = 35;
    private Coroutine _rotAnimCoroutine;
    private Sequence _rotAnimSequence;
    public Collider2D bossCollider;
    private readonly Collider2D[] _results = new Collider2D[4];
    public float damageValue = 8.2f;
    
    // Новый Init для босса
    public void Init(float hp, Color color, float scale, float bossDamage, System.Action onBossDeath)
    {
        CurrentData = new EnemyData();
        CurrentData.Health = hp;
        CurrentData.DamagePerHit = bossDamage;
        CurrentData.DamagePerHit = 0;
        damageReciever.EnemyData = CurrentData;
        
        _onBossDeath = onBossDeath;
        // Инициализация view
        if (view is BossView bossView)
        {
            bossView.SetColor(color);
            bossView.SetScale(scale);
        }
        // Устанавливаем здоровье напрямую, без EnemyData
        SetBossHealth(hp);
        gameObject.SetActive(true);
        // Инициализация направления и камеры
        _direction = Random.insideUnitCircle.normalized;
        _mainCamera = Camera.main;
        
        StartRotAnim();
    }

    private void SetBossHealth(float hp)
    {
        // Используем приватные поля базового класса через рефлексию или делаем protected
        CurrentHealth = hp;
        IsDead = false;
        IsActive = true;
        view.SetHitValue(0);
        view.SetFillValue(0);
    }

    protected override void DoMove()
    {
        var transform1 = transform;
        
        var delta = _direction * speedBoss * Time.deltaTime;
        var position = transform1.position;
        
        position += new Vector3(delta.x, delta.y, 0);
        transform1.position = position;

        var screenPos = _mainCamera.WorldToScreenPoint(position);
        var bounced = RandomCursorInput.MoveBounce(screenPos, ref _direction, paddingMove);
        transform.position = _mainCamera.ScreenToWorldPoint(screenPos);
        
        if (bounced) _direction = _direction.normalized;
        
        // base.DoMove();
    }

    // Переопределяем смерть
    public override void Die(bool isCrit = false)
    {
        if (!IsActive) return;
        _onBossDeath?.Invoke();
        base.Die(isCrit);
    }

    // DOTween-анимация вращения босса
    private void StartRotAnim()
    {
        _rotAnimSequence?.Kill(); // Остановить предыдущую анимацию, если есть
        rotAnimation.ReturnToZero();
        _rotAnimSequence = DOTween.Sequence();
        var startPos = rotAnimation._zeroLocalPosition;
        var downPos = startPos + Vector3.down * rotAnimation.moveDownDistance;
        
        _rotAnimSequence.Append(rotAnimation.transform.DOLocalMove(downPos, rotAnimation.moveDownDuration).SetEase(Ease.Linear));
        _rotAnimSequence.Append(rotAnimation.transform.DOLocalMove(startPos, rotAnimation.returnDuration).SetEase(Ease.Linear).OnComplete(OnAction));
        _rotAnimSequence.AppendInterval(0.5f); // пауза между движениями
        _rotAnimSequence.SetLoops(-1);
    }

    private void OnAction()
    {
        // Проверяем, пересекается ли bossCollider с коллайдером курсора
        int count = Physics2D.OverlapCollider(bossCollider, ContactFilter2D.noFilter, _results);
        for (int i = 0; i < count; i++)
        {
            if (_results[i] == G.I.playerCursor.collider2D)
            {
                DoDamageBoss();
                return;
            }
        }
    }

    private void DoDamageBoss()
    {
        var finalDamage = G.I.HpManager.Reduce(damageValue, doBlink: true, ignoreShield: false);
        float totalShield = damageValue - finalDamage;
        float totalDamage = finalDamage;
        var damageResult = new DamageResult { PlayerDoDamage = true, TotalDamage = totalDamage, TotalShieldDamage = totalShield };
        
        if (damageResult.HasRecivedDamage)
            G.I.playerCursor.view.PulseRingColor(Color.red, 0.15f);

        if (damageResult.HasAnyDamage)
            G.I.playerCursor.view.hudView.AddDamage(damageResult, G.I.playerCursor.CurrentDamageScale);
    }
} 