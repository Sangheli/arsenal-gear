using UnityEngine;
using UnityEngine.UI;

public class UILocalizationMonobeh : MonoBehaviour
{
    private UILocalization _uiLocalization;
    public Text text;
    public string key;
    
    private void OnValidate()
    {
        text ??= GetComponent<Text>();
    }

    private void Awake()
    {
        _uiLocalization = new UILocalization { LocalizationKey = key, TextComponent = text };
    }

    private void Start()
    {
        G.I.Localizations.Add(_uiLocalization);
        _uiLocalization.UpdateText();
    }
}