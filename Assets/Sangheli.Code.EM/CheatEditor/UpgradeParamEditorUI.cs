using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeParamEditorUI : MonoBehaviour
{
    public Transform contentRoot; // Контейнер для строк
    public GameObject rowPrefab; // Префаб строки с InputField
    public Button saveButton, loadButton;

    private List<UpgradeParamRow> rows = new List<UpgradeParamRow>();

    void Start()
    {
        Init();
    }

    public void Init()
    {
        foreach (Transform child in contentRoot)
            Destroy(child.gameObject);
        rows.Clear();
        foreach (var kv in ParamData.Params)
        {
            var rowObj = Instantiate(rowPrefab, contentRoot);
            var row = rowObj.GetComponent<UpgradeParamRow>();
            row.Set(kv.Key, kv.Value.DisplayName, kv.Value.BasePrice);
            rows.Add(row);
        }
        saveButton.onClick.RemoveAllListeners();
        loadButton.onClick.RemoveAllListeners();
        saveButton.onClick.AddListener(Save);
        loadButton.onClick.AddListener(Load);
    }

    void Save()
    {
        var config = new UpgradeParamConfig();
        foreach (var row in rows)
        {
            config.Upgrades.Add(new UpgradeParamEntry
            {
                Key = row.ParamKey,
                BasePrice = row.GetBasePrice()
            });
        }
        UpgradeParamPrefsManager.Save(config);
        ParamData.ApplyConfig(config);
    }

    void Load()
    {
        var config = UpgradeParamPrefsManager.Load();
        if (config == null) return;
        foreach (var row in rows)
        {
            var found = config.Upgrades.FirstOrDefault(u => u.Key == row.ParamKey);
            if (found != null)
                row.SetBasePrice(found.BasePrice);
        }
        ParamData.ApplyConfig(config);
    }
} 