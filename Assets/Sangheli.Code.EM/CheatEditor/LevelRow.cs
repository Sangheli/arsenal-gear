using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class LevelRow : MonoBehaviour
{
    public InputField levelIdInput;
    public Transform enemyListRoot;
    public EnemyRow enemyRowPrefab;
    public Button addEnemyButton;

    private readonly List<EnemyRow> _enemyRows = new();

    void Awake()
    {
        addEnemyButton.onClick.AddListener(AddEnemy);
    }

    public void Set(LevelConfig config)
    {
        levelIdInput.text = config.levelId.ToString();

        // Переиспользуем или создаём строки
        for (int i = 0; i < config.enemies.Count; i++)
        {
            EnemyRow row;
            if (i < _enemyRows.Count)
            {
                row = _enemyRows[i];
                row.gameObject.SetActive(true);
            }
            else
            {
                row = Instantiate(enemyRowPrefab, enemyListRoot);
                _enemyRows.Add(row);
            }
            row.Set(config.enemies[i]);
        }

        // Скрываем лишние строки
        for (int i = config.enemies.Count; i < _enemyRows.Count; i++)
        {
            _enemyRows[i].gameObject.SetActive(false);
        }
    }

    public LevelConfig GetLevelConfig()
    {
        var config = new LevelConfig();
        int.TryParse(levelIdInput.text, out config.levelId);
        config.enemies = new List<LevelEnemyInfo>();
        foreach (var row in _enemyRows)
            config.enemies.Add(row.GetEnemyInfo());
        return config;
    }

    private void AddEnemy()
    {
        var rowObj = Instantiate(enemyRowPrefab, enemyListRoot);
        var row = rowObj.GetComponent<EnemyRow>();
        row.Set(new LevelEnemyInfo());
        _enemyRows.Add(row);
    }
} 