using UnityEngine;

public class PrestigeLevelController : MonoBehaviour
{
    public PrestigeBar prestigeBar;

    private PrestigeData _prestigeData;
    private DataSave _save;
    private float _currentTime;
    private bool _active;
    private float _prestigeTime;
    
    public void Toggle(bool state)
    {
        _active = state;
        gameObject.SetActive(state);
        prestigeBar.gameObject.SetActive(state);
        
        if (!state) return;

        _currentTime = 0;
        _prestigeTime = G.I.prestigeManager.GetTime();
        _save = G.I.SaveProvider.Get();
        _prestigeData = G.I.prestigeManager.GetLastPrestige(_save.prestige);
        prestigeBar.Fill(_currentTime,_prestigeTime);
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

        if (_currentTime >= _prestigeTime)
        {
            _currentTime = _prestigeTime;
            _active = false;
            OnEnd();
        }
        
        prestigeBar.Fill(_currentTime,_prestigeTime);
    }

    private void OnEnd()
    {
        G.I.prestigeManager.IncreasePrestige(1);
        G.I.prestigeManager.AddPrestigePoint(1);
        G.I.StopRunNewPrestige();
    }
}