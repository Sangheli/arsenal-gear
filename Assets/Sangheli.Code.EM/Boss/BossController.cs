using UnityEngine;
using DG.Tweening;

public class BossController : EnemyController
{
    private System.Action _onBossDeath;
    private Vector2 _direction = Vector2.zero;
    private Camera _mainCamera;
    public float paddingMove = 35;
    private Coroutine _rotAnimCoroutine;
    private Sequence _rotAnimSequence;
    
    // Новый Init для босса
    public void Init(float hp, Color color, float bossDamage, System.Action onBossDeath)
    {
        view.render.blockSpriteReplace = true;
        CurrentData = G.I.Data.enemyContainer.List[5];
        CurrentData.SetBaseHP(hp);
        CurrentData.SetBaseDamage(0);
        DamageValue = bossDamage;
        fireRateMin = CurrentData.FireRateMin;
        damageReciever.EnemyData = CurrentData;
        
        _onBossDeath = onBossDeath;
        // Инициализация view
        if (view is BossView bossView)
        {
            bossView.SetColor(color);
            bossView.SetScale(CurrentData.viewScale.x);
            
            bossView.hitVfx.SetColor(color, Color.clear);
            bossView.hitVfx.SetScale(CurrentData.hitVfx);
            bossView.deathVfx.SetScale(CurrentData.deathVfxScale);
        }
        
        // Устанавливаем здоровье напрямую, без EnemyData
        SetBossHealth(hp);
        gameObject.SetActive(true);
        // Инициализация направления и камеры
        _direction = Random.insideUnitCircle.normalized;
        _mainCamera = Camera.main;
    }
    
    private void SetBossHealth(float hp)
    {
        // Используем приватные поля базового класса через рефлексию или делаем protected
        HpManager.Init(unfillTime,true,hp);
        IsDead = false;
        IsActive = true;
        view.SetHitValue(0);
        view.SetFillValue(0);
    }

    protected override void DoMove()
    {
        if(!CanAttack)
            return;
        
        // Check if player HP is zero or less
        if (G.I.HpManager.RoundHp <= 0)
        {
            CanAttack = false;
            return;
        }
        
        var transform1 = transform;
        
        var delta = _direction * CurrentData.Speed * Time.deltaTime;
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
    public override void Die(bool isCrit, bool killedByPlayer)
    {
        if (!IsActive) return;
        _onBossDeath?.Invoke();
        base.Die(isCrit, killedByPlayer);
    }
} 