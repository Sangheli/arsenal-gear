using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class LevelDataEditorUI : MonoBehaviour
{
    public Transform contentRoot; // Контейнер для строк уровней
    public LevelRow levelRowPrefab; // Префаб строки уровня
    public Button saveButton, loadButton, closeButton;

    private readonly List<LevelRow> _rows = new();
    private readonly List<LevelRow> _rowPool = new(); // Пул для переиспользования строк
    private List<LevelConfig> _workingCopy = new();

    private void Awake()
    {
        saveButton.onClick.AddListener(Save);
        loadButton.onClick.AddListener(Load);
        closeButton.onClick.AddListener(OnClose);
    }

    public void Init()
    {
        // Возвращаем все строки в пул и скрываем их
        foreach (var row in _rows)
        {
            row.gameObject.SetActive(false);
            _rowPool.Add(row);
        }
        _rows.Clear();

        _workingCopy = LevelData.Levels.Select(CloneLevel).ToList();
        for (int i = 0; i < _workingCopy.Count; i++)
        {
            LevelRow row;
            if (_rowPool.Count > 0)
            {
                row = _rowPool[_rowPool.Count - 1];
                _rowPool.RemoveAt(_rowPool.Count - 1);
                row.gameObject.SetActive(true);
            }
            else
            {
                row = Instantiate(levelRowPrefab, contentRoot);
            }
            row.transform.SetParent(contentRoot, false);
            row.Set(_workingCopy[i]);
            _rows.Add(row);
        }
    }

    private void OnClose()
    {
        gameObject.SetActive(false);
    }

    private void Save()
    {
        var config = new LevelDataConfig { Levels = _rows.Select(r => r.GetLevelConfig()).ToList() };
        var json = JsonUtility.ToJson(config);
        PlayerPrefs.SetString("leveldataprefs", json);
        PlayerPrefs.Save();
        
        LevelData.Levels.Clear();
        LevelData.Levels.AddRange(_rows.Select(r => r.GetLevelConfig()));
    }

    private void Load()
    {
        if (!PlayerPrefs.HasKey("leveldataprefs")) return;
        var json = PlayerPrefs.GetString("leveldataprefs");
        var config = JsonUtility.FromJson<LevelDataConfig>(json);
        if (config == null) return;
        _workingCopy = config.Levels.Select(CloneLevel).ToList();
        Init();
    }

    private static LevelConfig CloneLevel(LevelConfig src) =>
        new()
        {
            levelId = src.levelId,
            enemies = src.enemies.Select(e => new LevelEnemyInfo
            {
                enemyId = e.enemyId,
                spawnChance = e.spawnChance, // (0..100)
                spawnRate = e.spawnRate
            }).ToList()
        };

    [System.Serializable]
    private class LevelDataConfig
    {
        public List<LevelConfig> Levels;
    }
} 