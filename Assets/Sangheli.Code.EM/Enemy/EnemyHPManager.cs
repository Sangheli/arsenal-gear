using UnityEngine;

public class EnemyHPManager : MonoBehaviour
{
    public HalfBarView HpBarView;
    private float _currentHealth;
    private float _fullHp;
    
    public void Init(float unfillTime, bool show, float hp)
    {
        _currentHealth = _fullHp = hp;
        HpBarView.UnFillTime = unfillTime;
        HpBarView.gameObject.SetActive(show);
        HpBarView.Show(1,false,true);
    }

    public void Reset() => _currentHealth = 0;

    private void Show(float value) => HpBarView.Show(value,true,false);

    public void TakeDamage(float amount)
    {
        _currentHealth -= amount;
        if (_currentHealth <= 0)
            _currentHealth = 0;
        
        Show(_currentHealth/_fullHp);
    }
    
    public float GetHp() => _currentHealth;
}