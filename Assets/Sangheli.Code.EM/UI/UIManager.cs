using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public HealthBarUI healthBarUI;
    public ShieldBarUI shieldBarUI;
    public EnergyBarUI energyBarUI;
    
    [Header("Terminate Button")]
    public Button terminateButton;

    private void Awake()
    {
            terminateButton.onClick.AddListener(OnTerminateButtonClicked);
    }

    private void OnTerminateButtonClicked()
    {
        if (G.I != null && G.I.roundManager != null && G.I.roundManager.IsAlive)
            G.I.roundManager.TerminateRound();
    }

    public void OnStartRound()
    {
        healthBarUI?.Init();
        shieldBarUI?.Init();
        
        // Show/hide terminate button based on level config
        UpdateTerminateButtonVisibility();
    }

    private void UpdateTerminateButtonVisibility()
    {
        if (terminateButton == null) return;

        bool showButton = true; // Default value
        
        var save = G.I.SaveProvider?.Get();
        if (save != null && save.level < G.I.LevelDataConfig.Count())
        {
            var levelConfig = G.I.LevelDataConfig.Get(save.level);
            if (levelConfig != null)
                showButton = levelConfig.showTerminateButton;
        }
        
        terminateButton.gameObject.SetActive(showButton);
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

    public void Toggle(bool b)
    {
        gameObject.SetActive(b);
        
        // Hide terminate button when UI is toggled off
        if (!b && terminateButton != null)
            terminateButton.gameObject.SetActive(false);
    }
}