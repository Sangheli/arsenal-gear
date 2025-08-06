using System.Collections.Generic;
using UnityEngine;

namespace SangheliCore
{
    public class LocalizationBridge : MonoBehaviour
    {
        public static HashSet<IUILocalization> Localizations = new();
        private LocalizationManager _localizationManager;

        public void SetLanguage(string lang) => LocalizationManager.Lang = lang;
        public void OnLangSelected() => OnLangSelected(LocalizationManager.Lang);

        public void OnLangSelected(string lang)
        {
            _localizationManager = new LocalizationManager(lang);
            UpdateLocalization();
            G.I.OnFirstLoad();
        }
    
        public void UpdateLocalization()
        {
            foreach (var x in Localizations) x.UpdateText();
        }

        public string Get(string key) => _localizationManager?.Get(key);
    }
}