using UnityEngine;
using UnityEngine.UI;

public class CursorHudView : MonoBehaviour
{
    public DamageTextManager damageTextManager;
    public HealthBarUI hpGo;
    public Text hpLowText;
    private bool? _lowShowen;
    public Text aiText;

    public void Toggle(bool state, float scale, bool doShift)
    {
        if (_lowShowen == state)
            return;

        if(doShift) transform.localPosition = GetShift(scale);
        _lowShowen = state;
        hpGo.gameObject.SetActive(state);
        hpLowText.gameObject.SetActive(state);
        ToggleAIView(false);
    }

    private Vector3 GetShift(float scale) => new(0, 1.75f + scale / 2,10);

    public void ShowHpValue(float hp, float maxHp, float fillValue, bool doBlink)
    {
        if (_lowShowen == null || !_lowShowen.Value)
            return;
        
        hpGo.Show(hp, maxHp, fillValue, doBlink, false);
    }

    public void AddDamage(DamageResult damageResult, float currentDamageScale)
    {
        // damageTextManager.AddDamage(damageResult, currentDamageScale);
    }

    public void ToggleAIView(bool state) => aiText.gameObject.SetActive(state);
} 