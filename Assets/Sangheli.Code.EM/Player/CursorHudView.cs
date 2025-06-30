using UnityEngine;
using UnityEngine.UI;

public class CursorHudView : MonoBehaviour
{
    public DamageTextManager damageTextManager;
    public HealthBarUI hpGo;
    public Text hpLowText;
    private bool? _lowShowen;
    public Text aiText;

    public void Toggle(bool state)
    {
        if (_lowShowen == state)
            return;
        _lowShowen = state;
        if (hpGo != null) hpGo.gameObject.SetActive(state);
        if (hpLowText != null) hpLowText.gameObject.SetActive(state);
        ToggleAIView(false);
    }

    public void ShowHpValue(float hp, float maxHp, float fillValue)
    {
        if (_lowShowen == null || !_lowShowen.Value)
            return;
        if (hpGo != null)
            hpGo.ShowHp(hp, maxHp, fillValue, false);
    }

    public void AddDamage(float damage, float shieldDamage = 0f)
    {
        if (damageTextManager != null)
            damageTextManager.AddDamage(damage, shieldDamage);
    }

    public void ToggleAIView(bool state)
    {
        aiText.gameObject.SetActive(state);
    }
} 