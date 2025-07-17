using UnityEngine;

public class BossController : EnemyController
{
    private System.Action _onBossDeath;
    private Vector2 _direction = Vector2.zero;
    public float speedBoss = 2f;
    private Camera _mainCamera;
    public float paddingMove = 35;

    // Новый Init для босса
    public void Init(float hp, Color color, float scale, float bossDamage, System.Action onBossDeath)
    {
        _currentData = new EnemyData();
        _currentData.Health = hp;
        _currentData.DamagePerHit = bossDamage;
        damageReciever.EnemyData = _currentData;
        
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
    }

    private void SetBossHealth(float hp)
    {
        // Используем приватные поля базового класса через рефлексию или делаем protected
        _currentHealth = hp;
        _isDead = false;
        _isActive = true;
        view.SetHitValue(0);
        view.SetFillValue(0);
    }

    public override void DoMove()
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
        if (!_isActive) return;
        _onBossDeath?.Invoke();
        base.Die(isCrit);
    }
} 