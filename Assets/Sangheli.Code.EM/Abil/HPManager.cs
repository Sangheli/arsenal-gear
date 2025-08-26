using System;
using System.Linq;
using UnityEngine;

public class HPManager
{
    public float RoundHp;
    
    private const float DelimSpeed = 60f;
    private const float Tolerance = 0.001f;
    private const float MinHpValueToShow = 5;
    private const float MinHpValueToShowPrc = 0.3f;
    private float _lastUpdateHp;
    private float _maxHp;

    private bool _doBlink;

    private DataSave _save;
    
    public static readonly string[] Keys = 
    {
        "hp",
        "hp1",
        "hp2",
        "hp3",
        "hp4",
        "hp5",
    };

    public void OnStartRound()
    {
        _save = G.I.SaveProvider.Get();
        RoundHp = _maxHp = Keys.Sum(x => _save.GetParamContainer().GetValue(x));
        G.I.uiManager.ShowHp(RoundHp, _maxHp, 1, false, startValue: true );
    }

    public float Reduce(float reduceValue, bool doBlink, bool ignoreShield)
    {
        if (reduceValue <= 0)
            return reduceValue;

        if (doBlink)
            _doBlink = true;

        var actualDamage = ignoreShield
            ? reduceValue
            : G.I.ShieldManager.ProcessDamage(reduceValue);

        if (actualDamage <= 0)
            _doBlink = false;
        
        RoundHp -= Mathf.Abs(actualDamage);

        if (RoundHp < 0)
            RoundHp = 0;

        return actualDamage;
    }

    public void UpdateHp()
    {
        // Округляем roundHp до 1 знака после запятой
        var roundedHp = Mathf.Round(RoundHp * DelimSpeed) / DelimSpeed;

        if (Math.Abs(_lastUpdateHp - roundedHp) < Tolerance)
            return;

        _lastUpdateHp = roundedHp;
        G.I.uiManager.ShowHp(roundedHp, _maxHp, roundedHp / _maxHp, _doBlink, startValue: false);
        _doBlink = false;

        var step = _maxHp * MinHpValueToShowPrc;
        step = Mathf.Max(step, MinHpValueToShow);
        
        // G.I.playerCursor.view.hudView.Toggle(roundedHp < step,G.I.playerCursor.CurrentDamageScale, true);
    }
} 