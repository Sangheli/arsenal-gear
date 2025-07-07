using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeParamEditorUI : MonoBehaviour
{
    public Transform contentRoot; // Контейнер для строк
    public UpgradeParamRow rowPrefab; // Префаб строки с InputField
    public Button saveButton, loadButton;

    private readonly List<UpgradeParamRow> _rows = new();

    private void Start() => Init();

    private void Init()
    {
        foreach (Transform child in contentRoot)
            Destroy(child.gameObject);
    
        _rows.Clear();
        foreach (var kv in ParamData.Params)
        {
            var row = Instantiate(rowPrefab, contentRoot);
            row.Set(kv.Key, kv.Value.DisplayName, kv.Value.BasePrice);
            _rows.Add(row);
        }
        saveButton.onClick.RemoveAllListeners();
        loadButton.onClick.RemoveAllListeners();
        saveButton.onClick.AddListener(Save);
        loadButton.onClick.AddListener(Load);
    }

    private void Save()
    {
        var config = new UpgradeParamConfig();
        foreach (var row in _rows)
        {
            config.Upgrades.Add(new UpgradeParamEntry
            {
                Key = row.paramKey,
                BasePrice = row.GetBasePrice()
            });
        }
        UpgradeParamPrefsManager.Save(config);
        ParamData.ApplyConfig(config);
    }

    private void Load()
    {
        var config = UpgradeParamPrefsManager.Load();
        if (config == null) return;
        foreach (var row in _rows)
        {
            var found = config.Upgrades.FirstOrDefault(u => u.Key == row.paramKey);
            if (found != null)
                row.SetBasePrice(found.BasePrice);
        }
        
        ParamData.ApplyConfig(config);
    }
} 