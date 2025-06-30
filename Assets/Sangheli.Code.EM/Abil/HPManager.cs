using System;
using UnityEngine;

public class HPManager
{
    public float RoundHp;
    
    private const float Tolerance = 0.001f;
    private const float MinHpValueToShow = 5;
    private float _lastUpdateHp;
    private float _maxHp;

    private bool _doBlink;

    private DataSave _save;
    
    public void StartRound()
    {
        _save = G.I.SaveProvider.Get();
        RoundHp = _maxHp = _save.GetParamContainer().GetValue("hp");
        G.I.uiManager.ShowHp(RoundHp, _maxHp, 1, false);
    }

    public void Reduce(float reduceValue, bool doBlink, bool ignoreShield)
    {
        if (reduceValue <= 0)
            return;

        if (doBlink) _doBlink = true;
        var actualDamage = ignoreShield
            ? reduceValue
            : G.I.ShieldManager.ProcessDamage(reduceValue);

        RoundHp -= Mathf.Abs(actualDamage);

        if (RoundHp < 0)
            RoundHp = 0;
    }

    public float ReduceHpWithReturn(float reduceValue, bool doBlink, bool ignoreShield)
    {
        if (reduceValue <= 0)
            return 0f;
        if (doBlink) _doBlink = true;
        var actualDamage = ignoreShield
            ? reduceValue
            : G.I.ShieldManager.ProcessDamage(reduceValue);
        RoundHp -= Mathf.Abs(actualDamage);
        if (RoundHp < 0)
            RoundHp = 0;
        return actualDamage;
    }

    public void UpdateHp()
    {
        // Округляем roundHp до 1 знака после запятой
        var roundedHp = Mathf.Round(RoundHp * 10f) / 10f;

        if (Math.Abs(_lastUpdateHp - roundedHp) < Tolerance)
            return;

        _lastUpdateHp = roundedHp;
        G.I.uiManager.ShowHp(roundedHp, _maxHp, roundedHp / _maxHp, _doBlink);
        _doBlink = false;

        G.I.playerCursor.view.hudView.Toggle(roundedHp < MinHpValueToShow);
    }
} 