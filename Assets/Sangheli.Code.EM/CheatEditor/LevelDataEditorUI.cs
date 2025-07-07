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
    private List<LevelConfig> _workingCopy = new();

    private void Awake()
    {
        saveButton.onClick.RemoveAllListeners();
        loadButton.onClick.RemoveAllListeners();
        closeButton.onClick.RemoveAllListeners();
        saveButton.onClick.AddListener(Save);
        loadButton.onClick.AddListener(Load);
        closeButton.onClick.AddListener(OnClose);
    }

    public void Init()
    {
        foreach (Transform child in contentRoot)
            Destroy(child.gameObject);
        
        _rows.Clear();
        _workingCopy = LevelData.Levels.Select(CloneLevel).ToList();
        foreach (var level in _workingCopy)
        {
            var row = Instantiate(levelRowPrefab, contentRoot);
            row.Set(level);
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