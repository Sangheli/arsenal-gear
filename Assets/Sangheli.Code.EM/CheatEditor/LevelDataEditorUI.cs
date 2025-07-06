using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Assets.Sangheli.Code.EM.Game; // если потребуется для доступа к LevelConfig

public class LevelDataEditorUI : MonoBehaviour
{
    public Transform contentRoot; // Контейнер для строк уровней
    public GameObject levelRowPrefab; // Префаб строки уровня
    public Button saveButton, loadButton, applyButton;

    private List<LevelRow> rows = new List<LevelRow>();
    private List<LevelConfig> _workingCopy = new List<LevelConfig>();

    void Start()
    {
        Init();
    }

    public void Init()
    {
        foreach (Transform child in contentRoot)
            Destroy(child.gameObject);
        rows.Clear();
        _workingCopy = LevelData.Levels.Select(CloneLevel).ToList();
        foreach (var level in _workingCopy)
        {
            var rowObj = Instantiate(levelRowPrefab, contentRoot);
            var row = rowObj.GetComponent<LevelRow>();
            row.Set(level);
            rows.Add(row);
        }
        saveButton.onClick.RemoveAllListeners();
        loadButton.onClick.RemoveAllListeners();
        applyButton.onClick.RemoveAllListeners();
        saveButton.onClick.AddListener(Save);
        loadButton.onClick.AddListener(Load);
        applyButton.onClick.AddListener(Apply);
    }

    void Save()
    {
        var config = new LevelDataConfig { Levels = rows.Select(r => r.GetLevelConfig()).ToList() };
        string json = JsonUtility.ToJson(config);
        PlayerPrefs.SetString("leveldataprefs", json);
        PlayerPrefs.Save();
    }

    void Load()
    {
        if (!PlayerPrefs.HasKey("leveldataprefs")) return;
        string json = PlayerPrefs.GetString("leveldataprefs");
        var config = JsonUtility.FromJson<LevelDataConfig>(json);
        if (config == null) return;
        _workingCopy = config.Levels.Select(CloneLevel).ToList();
        Init();
    }

    void Apply()
    {
        LevelData.Levels.Clear();
        LevelData.Levels.AddRange(rows.Select(r => r.GetLevelConfig()));
    }

    private LevelConfig CloneLevel(LevelConfig src)
    {
        return new LevelConfig
        {
            levelId = src.levelId,
            enemies = src.enemies.Select(e => new LevelEnemyInfo
            {
                enemyId = e.enemyId,
                spawnChance = e.spawnChance,
                spawnRate = e.spawnRate
            }).ToList()
        };
    }

    [System.Serializable]
    private class LevelDataConfig
    {
        public List<LevelConfig> Levels;
    }
} 