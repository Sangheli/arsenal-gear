using System;
using System.Globalization;
using UnityEngine;

public class SaveSystem
{
    public const int SaveSlot = 0;
    private const string SaveKeyPrefix = "SaveSlot_";
    
    public static void SaveSilent() => Save(false, EndRoundFadeEnum.None);

    public static void Save(bool sendAnalytics, EndRoundFadeEnum endRoundType)
    {
        PlayerPrefs.SetString(SaveKeyPrefix + SaveSlot, DateTime.Now.ToString(CultureInfo.InvariantCulture));
        
        var save = G.I.SaveProvider.Get();
        switch (endRoundType)
        {
            case EndRoundFadeEnum.RoundWin:
                save.wins++;
                break;
            case EndRoundFadeEnum.RoundFail:
                save.losses++;
                break;
        }
        
        G.I.SaveProvider.Save(SaveSlot, sendAnalytics, endRoundType);
    }

    public static int GetSlot() => SaveSlot;
    public static string GetKey() => $"{SaveKeyPrefix}{SaveSlot}";
}