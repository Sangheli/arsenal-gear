using System.Collections.Generic;
using UnityEngine;

namespace SangheliCore
{
    public class LocalizationBridge : MonoBehaviour
    {
        public LocalizationSelector localizationSelector;
        
        public readonly HashSet<IUILocalization> Localizations = new();
        private LocalizationManager _localizationManager;

        public void SetLanguage(string lang)
        {
            LocalizationManager.Lang = lang;
            UpdateLocalization();
        }

        public void ShowLangSelector()
        {
            localizationSelector.gameObject.SetActive(true);
        }

        public void OnLangSelected(string lang)
        {
            _localizationManager = new LocalizationManager(lang);
            UpdateLocalization();
            localizationSelector.gameObject.SetActive(false);
            
            G.I.OnFirstLoad();
        }
    
        public void UpdateLocalization()
        {
            foreach (var x in Localizations) x.UpdateText();
        }

        public string Get(string key) => _localizationManager?.Get(key);
    }
}