using UnityEngine;
using Newtonsoft.Json;
using System;

public class SaveManager<T> where T : new()
{
    private readonly string _saveKey;

    private readonly JsonSerializerSettings _settings = new()
    {
        MissingMemberHandling = MissingMemberHandling.Ignore, // игнорировать лишние поля
        NullValueHandling = NullValueHandling.Include,         // включать null
        ObjectCreationHandling = ObjectCreationHandling.Replace,
        DefaultValueHandling = DefaultValueHandling.Populate, // заполнять дефолтами
        Error = (sender, args) =>
        {
            // Debug.LogWarning($"[SaveManager] JSON parse error: {args.ErrorContext.Error.Message}");
            args.ErrorContext.Handled = true;
        }
    };

    public SaveManager(string key) => _saveKey = key;
    public bool Exists(int slot = 0) => PlayerPrefs.HasKey(GetSlotKey(slot));

    public T Load(int slot = 0)
    {
        string saveKey = GetSlotKey(slot);
        var hasKey = PlayerPrefs.HasKey(saveKey);
        if (!hasKey) return new T();

        string json = PlayerPrefs.GetString(saveKey);
        try
        {
            var data = JsonConvert.DeserializeObject<T>(json, _settings);
            return data ?? new T();
        }
        catch (Exception e)
        {
            // Debug.LogError($"[SaveManager] Exception during load: {e.Message}");
            return new T();
        }
    }

    public string Save(T data, int slot = 0)
    {
        string saveKey = GetSlotKey(slot);
        string json = JsonConvert.SerializeObject(data, Formatting.Indented, _settings);
        PlayerPrefs.SetString(saveKey, json);
        PlayerPrefs.Save();
        return json;
    }

    public void Delete(int slot = 0) => PlayerPrefs.DeleteKey(GetSlotKey(slot));
    private string GetSlotKey(int slot) => $"{_saveKey}_{slot}";
}
