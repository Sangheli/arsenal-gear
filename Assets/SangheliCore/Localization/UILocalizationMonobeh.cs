using UnityEngine;
using UnityEngine.UI;

namespace SangheliCore
{
    public class UILocalizationMonobeh : MonoBehaviour, IUILocalization
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
            G.I.localization.Localizations.Add(this);
            UpdateText();
        }

        public void UpdateText()
        {
            _uiLocalization.UpdateText();
        }
    }
}