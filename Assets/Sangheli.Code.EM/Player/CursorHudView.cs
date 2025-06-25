using UnityEngine;

public class CursorHudView : MonoBehaviour
{
    public DamageTextManager damageTextManager;
    public HealthBarUI hpGo;
    public UnityEngine.UI.Text hpLowText;
    private bool? _lowShowen;

    public void Toggle(bool state)
    {
        if (_lowShowen == state)
            return;
        _lowShowen = state;
        if (hpGo != null) hpGo.gameObject.SetActive(state);
        if (hpLowText != null) hpLowText.gameObject.SetActive(state);
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
} 