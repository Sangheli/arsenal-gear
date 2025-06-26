using System;
using System.Linq;
using UnityEngine;

public class ShieldManager
{
    public float CurrentShieldEnergy { get; set; }
    private float MaxShieldEnergy { get; set; }
    private float ShieldRegenRate { get; set; }
    private bool _isShieldAvailable;

    private bool _doBlink;
    private float _lastValue;
    private const float Tolerance = 0.001f;

    public void StartRound()
    {
        var save = G.I.SaveProvider.Get();
        var paramContainer = save.GetParamContainer();
        var availableParams = paramContainer.GetAvailableParamKeys(save.prestige);
        _isShieldAvailable = availableParams.Contains("shieldCapacity");
        
        if (_isShieldAvailable)
        {
            MaxShieldEnergy = paramContainer.GetValue("shieldCapacity", false);
            ShieldRegenRate = paramContainer.GetValue("shieldRegenRate", false);
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
    }
    
    public float ProcessDamage(float incomingDamage)
    {
        if (!_isShieldAvailable || CurrentShieldEnergy < incomingDamage)
            return incomingDamage;

        CurrentShieldEnergy -= incomingDamage;
        _doBlink = true;
        return 0; // Весь урон поглощен
    }

    public bool IsShieldAvailable() => _isShieldAvailable;
    
    public float GetCurrentShieldEnergy() => _isShieldAvailable ? CurrentShieldEnergy : 0f;
    
    public float GetMaxShieldEnergy() => _isShieldAvailable ? MaxShieldEnergy : 0f;

    public void LateUpdate()
    {
        if (_isShieldAvailable)
        {
            UpdateShieldRegeneration();
        }
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
        // Округляем roundHp до 1 знака после запятой
        var rounded = Mathf.Round(CurrentShieldEnergy * 1000f) / 1000f;
        if (Math.Abs(_lastValue - rounded) < Tolerance)
            return;
        
        _lastValue = rounded;
        if (!G.I.uiManager.ShowShield(rounded, MaxShieldEnergy, rounded / MaxShieldEnergy, _doBlink)) 
            _lastValue = -1;

        _doBlink = false;
    }
} 