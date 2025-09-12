using UnityEngine;
using DG.Tweening;

public class BossController : EnemyController
{
    private System.Action _onBossDeath;
    private Camera _mainCamera;
    public float paddingMove = 35;
    private Coroutine _rotAnimCoroutine;
    private Sequence _rotAnimSequence;
    private readonly AUnitMove _move = new FlyMove();
    
    // Новый Init для босса
    public void Init(float hp, Color color, float bossDamage, System.Action onBossDeath)
    {
        view.render.blockSpriteReplace = true;
        CurrentData = G.I.Data.enemyContainer.List[5];
        CurrentData.SetBaseHP(hp);
        CurrentData.SetBaseDamage(0);
        DamageValue = bossDamage;
        damageReciever.EnemyData = CurrentData;

        var color1 = EnemyData.GetRandomColor();
        
        _onBossDeath = onBossDeath;
        // Инициализация view
        if (view is BossView bossView)
        {
            bossView.SetColor(Color.white, color1);
            bossView.SetScale(CurrentData.viewScale.x);
            
            // bossView.hitVfx.SetColor(color1, Color.clear);
            bossView.hitVfx.SetScale(CurrentData.hitVfx);
            bossView.deathVfx.SetScale(CurrentData.deathVfxScale);
        }
        
        // Устанавливаем здоровье напрямую, без EnemyData
        SetBossHealth(hp);
        gameObject.SetActive(true);
        _mainCamera = Camera.main;
        _move.Init(transform, CurrentData.Speed, _mainCamera, Random.insideUnitCircle.normalized, paddingMove, false, view.render.SetDirection);
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
        
        if (G.I.HpManager.RoundHp <= 0)
        {
            CanAttack = false;
            return;
        }
        
        _move.DoMove();
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