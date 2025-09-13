using UnityEngine.UI;

namespace SangheliCore
{
    public class UILocalization : IUILocalization
    {
        public string LocalizationKey;
        public Text TextComponent;

        public void UpdateText()
        {
            if(G.I==null || G.I.localization == null)
                return;
        
            TextComponent.text = G.I.localization.Get(LocalizationKey);
        }
    }
}