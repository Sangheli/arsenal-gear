using UnityEngine.UI;

public class UILocalization
{
    public string localizationKey;
    public Text textComponent;

    public void UpdateText() => textComponent.text = G.I.LocalizationManager.GetLocalizedValue(localizationKey);
}