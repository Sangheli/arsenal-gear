using System;
using System.Linq;
using UnityEngine;

public class ShieldManager
{
    private const float Tolerance = 0.001f;
    private float _lastValue;
    private float CurrentShieldEnergy { get; set; }
    private float MaxShieldEnergy { get; set; }
    private float ShieldRegenRate { get; set; }
    
    private bool _isShieldAvailable;
    private bool _doBlink;

    public void OnStartRound()
    {
        var save = G.I.SaveProvider.Get();
        var paramContainer = save.GetParamContainer();
        var availableParams = paramContainer.GetAllParamKeys();
        var isParamAvailable = availableParams.Contains("shieldCapacity");
        
        // Проверяем не только доступность, но и значение параметра
        var shieldCapacityValue = 0f;
        if (isParamAvailable)
            shieldCapacityValue = paramContainer.GetValue("shieldCapacity");
        
        // Щит доступен только если параметр доступен И его значение больше 0
        _isShieldAvailable = isParamAvailable && shieldCapacityValue > 0f;
        
        if (_isShieldAvailable)
        {
            MaxShieldEnergy = shieldCapacityValue;
            ShieldRegenRate = paramContainer.GetValue("shieldRegenRate");
            CurrentShieldEnergy = MaxShieldEnergy;
        }
        else
        {
            MaxShieldEnergy = 0f;
            ShieldRegenRate = 0f;
            CurrentShieldEnergy = 0f;
        }
        
        _lastValue = -1;
        _doBlink = false;
        Show();
        
        G.I.uiManager.shieldBarUI.ToggleShieldView(_isShieldAvailable);
    }
    
    public float ProcessDamage(float incomingDamage)
    {
        if (!_isShieldAvailable || CurrentShieldEnergy <= 0f)
            return incomingDamage;

        float absorbed = Mathf.Min(CurrentShieldEnergy, incomingDamage);
        CurrentShieldEnergy -= absorbed;
        _doBlink = true;
        return incomingDamage - absorbed; // Возвращаем остаток урона, если щит не смог поглотить всё
    }

    public void LateUpdate()
    {
        if (_isShieldAvailable) UpdateShieldRegeneration();
        Show();
    }
    
    private void UpdateShieldRegeneration()
    {
        if (CurrentShieldEnergy >= MaxShieldEnergy) return;
        CurrentShieldEnergy += ShieldRegenRate * Time.deltaTime;
        CurrentShieldEnergy = Mathf.Min(CurrentShieldEnergy, MaxShieldEnergy);
    }
    
    private void Show()
    {
        if(!_isShieldAvailable)
            return;
        
        // Округляем roundHp до 1 знака после запятой
        var rounded = Mathf.Round(CurrentShieldEnergy * 1000f) / 1000f;
        if (Math.Abs(_lastValue - rounded) < Tolerance)
            return;
        
        _lastValue = rounded;
        if (!G.I.uiManager.shieldBarUI.Show(rounded, MaxShieldEnergy, rounded / MaxShieldEnergy, _doBlink)) 
            _lastValue = -1;

        _doBlink = false;
    }
} 