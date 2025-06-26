using UnityEngine;

public class PrestigeLevelController : MonoBehaviour
{
    public PrestigeBar prestigeBar;

    private PrestigeData _prestigeData;
    private DataSave _save;
    private float _currentTime;
    private bool _active;
    
    public void Toggle(bool state)
    {
        _active = state;
        gameObject.SetActive(state);
        prestigeBar.gameObject.SetActive(state);
        
        if (!state) return;

        _currentTime = 0;
        _save = G.I.SaveProvider.Get();
        _prestigeData = G.I.prestigeManager.GetLastPrestige(_save.prestige);
        prestigeBar.Fill(_currentTime,_prestigeData.bossTime);
    }

    public void ToggleActive(bool state)
    {
        _active = state;
    }

    private void Update()
    {
        if(!_active)
            return;

        _currentTime += Time.deltaTime;

        if (_currentTime >= _prestigeData.bossTime)
        {
            _currentTime = _prestigeData.bossTime;
            _active = false;
            OnEnd();
        }
        
        prestigeBar.Fill(_currentTime,_prestigeData.bossTime);
    }

    private void OnEnd()
    {
        G.I.prestigeManager.IncreasePrestige(1);
        G.I.StopRunNewPrestige();
    }
}