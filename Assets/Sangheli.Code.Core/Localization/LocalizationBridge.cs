using System.Collections.Generic;
using UnityEngine;

namespace SangheliCore
{
    public class LocalizationBridge : MonoBehaviour
    {
        public static string LangStatic = "en";
        
        public bool useLocSelector = false;
        public LocalizationSelector localizationSelector;
        
        public static HashSet<IUILocalization> Localizations = new();
        private LocalizationManager _localizationManager;

        public void SetLanguage(string lang)
        {
            LangStatic = lang;
        }

        public void ShowLangSelector(string lang)
        {
            if(useLocSelector)
                localizationSelector.gameObject.SetActive(true);
            else
                OnLangSelected(lang);
        }

        public void OnLangSelected(string lang)
        {
            if (string.IsNullOrEmpty(lang))
                lang = LangStatic;
            
            _localizationManager = new LocalizationManager(lang);
            UpdateLocalization();
            
            if(useLocSelector)
                localizationSelector.gameObject.SetActive(false);
            
            _ = G.I.OnFirstLoad().LogExceptions();
        }
    
        public void UpdateLocalization()
        {
            foreach (var x in Localizations) x.UpdateText();
        }

        public string Get(string key) => _localizationManager?.Get(key);
    }
}