using UnityEngine;

public class EnemyHPManager : MonoBehaviour
{
    public HalfBarView HpBarView;
    private int _currentLevel;//unused
    private int _fullLevel;//unused
    private float _currentHealth;
    private float _fullHp;
    
    public void Init(int hpLevel, float unfillTime, bool show, float hp)
    {
        _currentHealth = _fullHp = hp;
        _currentLevel = _fullLevel = hpLevel;
        HpBarView.UnFillTime = unfillTime;
        HpBarView.gameObject.SetActive(show);
        HpBarView.Show(1,false,true);
    }

    public void Reset() => _currentHealth = 0;

    private void Show(float value) => HpBarView.Show(value,true,false);

    //unused
    public void UpdateColor()
    {
        var fill = G.I.Data.hpBaColorContainer.GetColor(_currentLevel);
        var back = G.I.Data.hpBaColorContainer.GetColor(_currentLevel-1);
        HpBarView.runningBarImage.SetColor(fill, back);
    }

    public void TakeDamage(float amount)
    {
        _currentHealth -= amount;
        if (_currentHealth <= 0)
            _currentHealth = 0;
        
        Show(_currentHealth/_fullHp);
    }
    
    public float GetHp() => _currentHealth;
}