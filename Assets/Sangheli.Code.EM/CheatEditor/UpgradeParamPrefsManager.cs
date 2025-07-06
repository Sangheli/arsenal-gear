using UnityEngine;

public static class UpgradeParamPrefsManager
{
    private const string Key = "paramdataprefs";

    public static void Save(UpgradeParamConfig data)
    {
        string json = JsonUtility.ToJson(data);
        PlayerPrefs.SetString(Key, json);
        PlayerPrefs.Save();
    }

    public static UpgradeParamConfig Load()
    {
        if (!PlayerPrefs.HasKey(Key)) return null;
        string json = PlayerPrefs.GetString(Key);
        return JsonUtility.FromJson<UpgradeParamConfig>(json);
    }
}