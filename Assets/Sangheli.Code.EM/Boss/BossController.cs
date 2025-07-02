using UnityEngine;

public class BossController : EnemyController
{
    private System.Action _onBossDeath;

    // Новый Init для босса
    public void Init(float hp, Color color, float scale, System.Action onBossDeath)
    {
        _currentData = new EnemyData();
        _currentData.Health = hp;
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

    // Переопределяем смерть
    public override void Die(bool isCrit = false)
    {
        if (!_isActive) return;
        _isDead = true;
        _isActive = false;
        gameObject.SetActive(false);
        _onBossDeath?.Invoke();
    }
} 