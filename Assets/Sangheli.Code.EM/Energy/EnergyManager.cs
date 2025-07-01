public class EnergyManager
{
    public int RoundEnergyCount;
    private int _lastUpdateEnergy;
    private DataSave _save;
    
    public void OnStartRound()
    {
        _save = G.I.SaveProvider.Get();
        _lastUpdateEnergy = -1;
        RoundEnergyCount = 0;
        G.I.uiManager.ShowEnergy(RoundEnergyCount);
    }
    
    public void OnGoToUpgradeView() => _save = G.I.SaveProvider.Get();
    public void CollectEnergy(int add =1) => RoundEnergyCount+=add;
    public bool CanBuyUpgrade(float price) => _save.totalEnergy >= price;

    public bool SpendEnergyOnUpgrade(float price)
    {
        if (!CanBuyUpgrade(price)) 
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
}