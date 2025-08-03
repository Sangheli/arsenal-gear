using UnityEngine;

public class LocalizationBridge : MonoBehaviour
{
    public static string Lang = "en";
    
    public void SetLanguage(string lang)
    {
        Lang = lang;
        G.I.UpdateLocalization();
    }
}