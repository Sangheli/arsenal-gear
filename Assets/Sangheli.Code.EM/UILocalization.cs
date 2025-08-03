using UnityEngine;
using UnityEngine.UI;

public class UILocalization
{
    public string LocalizationKey;
    public Text TextComponent;

    public void UpdateText()
    {
        TextComponent.text = G.I.LocalizationManager.GetLocalizedValue(LocalizationKey);
    }
}