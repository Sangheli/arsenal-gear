using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public HealthBarUI healthBarUI;
    public ShieldBarUI shieldBarUI;
    public EnergyBarUI energyBarUI;
    public ShakeSettings damageShakeSettings = new();
    
    [Header("Terminate Button")]
    public Button terminateButton;

    public void OnFirstLoad()
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
        if (save != null)
        {
            if (G.I.currentLevel != null)
                showButton = G.I.currentLevel.showTerminateButton;
        }
        
        terminateButton.gameObject.SetActive(showButton);
    }

    public void ShowEnergy(int value)
    {
        if(!gameObject.activeSelf)
            return;
        
        energyBarUI?.ShowEnergy(value);
    }

    public bool ShowHp(float hp, float fullValue, float fillValue, bool doBlink, bool startValue)
    {
        // Проверка флага loseHpDuringRound
        var showHp = true;
        var save = G.I.SaveProvider?.Get();
        if (save != null)
        {
            if (G.I.currentLevel is { showHpBar: false })
                showHp = false;
        }

        if (!showHp && healthBarUI != null)
        {
            healthBarUI.gameObject.SetActive(false);
            return false;
        }
        if (healthBarUI != null)
            healthBarUI.gameObject.SetActive(true);
        return healthBarUI != null && healthBarUI.Show(hp, fullValue, fillValue, doBlink, startValue: startValue);
    }

    public void PlayDamageShake()
    {
        healthBarUI?.Shake(damageShakeSettings);
        shieldBarUI?.Shake(damageShakeSettings);
    }

    public void Toggle(bool b)
    {
        gameObject.SetActive(b);
        
        // Hide terminate button when UI is toggled off
        if (!b && terminateButton != null)
            terminateButton.gameObject.SetActive(false);
    }
}