using System;
using UnityEngine;

public class ShieldManager
{
    public float CurrentShieldEnergy { get; set; }
    private float MaxShieldEnergy { get; set; }
    private float ShieldRegenRate { get; set; }

    private bool _doBlink;
    private float _lastValue;
    private const float Tolerance = 0.001f;

    public void StartRound()
    {
        var paramContainer = G.I.SaveProvider.Get().GetParamContainer();
        MaxShieldEnergy = paramContainer.GetValue("shieldCapacity", false);
        ShieldRegenRate = paramContainer.GetValue("shieldRegenRate", false);
        _lastValue = -1;
        CurrentShieldEnergy = MaxShieldEnergy;
        _doBlink = false;
        Show();
    }
    
    public float ProcessDamage(float incomingDamage)
    {
        if (CurrentShieldEnergy < incomingDamage)
            return incomingDamage;

        CurrentShieldEnergy -= incomingDamage;
        _doBlink = true;
        return 0; // Весь урон поглощен
    }

    public void LateUpdate()
    {
        UpdateShieldRegeneration();
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