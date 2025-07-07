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

    void Awake()
    {
        saveButton.onClick.AddListener(Save);
        loadButton.onClick.AddListener(Load);
    }

    private void Init()
    {
        int i = 0;
        var paramList = ParamData.Params.ToList();
        // Создаём недостающие строки
        while (_rows.Count < paramList.Count)
        {
            var row = Instantiate(rowPrefab, contentRoot);
            _rows.Add(row);
        }
        // Обновляем и активируем нужные строки
        for (; i < paramList.Count; i++)
        {
            var kv = paramList[i];
            var row = _rows[i];
            row.gameObject.SetActive(true);
            row.Set(kv.Key, kv.Value.DisplayName, kv.Value.BasePrice);
        }
        // Деактивируем лишние строки
        for (; i < _rows.Count; i++)
        {
            _rows[i].gameObject.SetActive(false);
        }
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