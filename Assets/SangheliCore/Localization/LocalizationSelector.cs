using UnityEngine;
using UnityEngine.UI;

namespace SangheliCore
{
    public class LocalizationSelector : MonoBehaviour
    {
        public Button bEn;
        public Button bRu;
        public Button bTr;

        private void Start()
        {
            bEn.GetComponentInChildren<Text>().text = "English";
            bRu.GetComponentInChildren<Text>().text = "Русский";
            bTr.GetComponentInChildren<Text>().text = "Türkçe";
        
            bEn.onClick.AddListener(()=>SetLang("en"));
            bRu.onClick.AddListener(()=>SetLang("ru"));
            bTr.onClick.AddListener(()=>SetLang("tr"));
        }

        private static void SetLang(string lang) => G.I.localization.OnLangSelected(lang);
    }
}