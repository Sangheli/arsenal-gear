using System;
using System.Linq;
using UnityEngine;

public class EnergyManager
{
    public int RoundEnergyCount;
    private const float EndWaitTime = 0.25f;
    private int _lastUpdateEnergy;
    private DataSave _save;
    private float _autoCollectPercent;
    
    public void StartRound()
    {
        _save = G.I.SaveProvider.Get();
        RoundEnergyCount = 0;
        _autoCollectPercent = _save.GetParamContainer().GetValue("autoCollectCoinsPercent");
        G.I.uiManager.ShowEnergy(RoundEnergyCount);
    }
    
    public void CollectEnergy(int add =1) => RoundEnergyCount+=add;
    public void OnEndRound() => _save.totalEnergy += RoundEnergyCount;
    public bool CanAffordUpgrade(float price) => _save.totalEnergy >= price;

    public bool SpendEnergyOnUpgrade(float price)
    {
        if (!CanAffordUpgrade(price)) 
            return false;
        
        _save.totalEnergy -= (int)price;
        return true;
    }

    public void LateUpdate()
    {
        UpdateEnergy();
    }

    private void UpdateEnergy()
    {
        if (_lastUpdateEnergy == RoundEnergyCount)
            return;

        _lastUpdateEnergy = RoundEnergyCount;
        G.I.uiManager.ShowEnergy(RoundEnergyCount);
    }
    
    public System.Collections.IEnumerator CollectAllEnergyOnScreen()
    {
        var autoCollectPercent = _autoCollectPercent;
        var energyViews = G.I.energySpawner.EnergyList;
        var toCollect = energyViews.Where(x => x.IsActive).ToList();

        // Выбираем только autoCollectPercent процентов из toCollect
        int countToCollect = Mathf.CeilToInt(toCollect.Count * autoCollectPercent/100f);
        var selectedToCollect = toCollect.OrderBy(x => Guid.NewGuid()).Take(countToCollect).ToList();

        if (selectedToCollect.Count <= 0)
            yield break;
        
        var player = G.I.playerCursor.transform;
        var collectRadius = G.I.playerCursor.CurrentDamageScale;
        
        foreach (var x in selectedToCollect) 
            x.Collect(player, 0.1f);
        
        while (selectedToCollect.Any(x => x.IsActive))
            yield return null;

        yield return new WaitForSeconds(EndWaitTime);
    }
}