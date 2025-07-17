using UnityEngine;

public class UIManager : MonoBehaviour
{
    public HealthBarUI healthBarUI;
    public ShieldBarUI shieldBarUI;
    public EnergyBarUI energyBarUI;

    public void OnStartRound()
    {
        healthBarUI?.Init();
        shieldBarUI?.Init();
    }

    public void ShowEnergy(int value)
    {
        if(!gameObject.activeSelf)
            return;
        
        energyBarUI?.ShowEnergy(value);
    }

    public bool ShowHp(float hp, float fullValue, float fillValue, bool doBlink)
    {
        // Проверка флага loseHpDuringRound
        var showHp = true;
        var save = G.I.SaveProvider?.Get();
        if (save != null && save.level < G.I.LevelDataConfig.Count())
        {
            var levelConfig = G.I.LevelDataConfig.Get(save.level);
            if (levelConfig is { loseHpDuringRound: false })
                showHp = false;
        }
        G.I.playerCursor.view.hudView.ShowHpValue(hp,fullValue,fillValue, doBlink);
        if (!showHp && healthBarUI != null)
        {
            healthBarUI.gameObject.SetActive(false);
            return false;
        }
        if (healthBarUI != null)
            healthBarUI.gameObject.SetActive(true);
        return healthBarUI != null && healthBarUI.ShowHp(hp, fullValue, fillValue, doBlink);
    }

    public bool ShowShield(float hp, float fullValue, float fillValue, bool doBlink) => 
        shieldBarUI != null && G.I.ShieldManager.IsShieldAvailable() && shieldBarUI.ShowShield(hp, fullValue, fillValue, doBlink);

    public void Toggle(bool b) => gameObject.SetActive(b);
}