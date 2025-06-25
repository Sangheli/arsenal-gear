using UnityEngine;

public class UIManager : MonoBehaviour
{
    public HealthBarUI healthBarUI;
    public ShieldBarUI shieldBarUI;
    public EnergyBarUI energyBarUI;

    public void StartRound()
    {
        healthBarUI?.Init();
        shieldBarUI?.Init();
    }

    public void ToggleShieldView() => shieldBarUI?.ToggleShieldView();

    public void ShowEnergy(int value) => energyBarUI?.ShowEnergy(value);

    public bool ShowHp(float hp, float fullValue, float fillValue, bool doBlink)
    {
        G.I.playerCursorHudView.ShowHpValue(hp,fullValue,fillValue);
        return healthBarUI != null && healthBarUI.ShowHp(hp, fullValue, fillValue, doBlink);
    }

    public bool ShowShield(float hp, float fullValue, float fillValue, bool doBlink) => 
        shieldBarUI != null && shieldBarUI.ShowShield(hp, fullValue, fillValue, doBlink);

    public void Toggle(bool b) => gameObject.SetActive(b);
}