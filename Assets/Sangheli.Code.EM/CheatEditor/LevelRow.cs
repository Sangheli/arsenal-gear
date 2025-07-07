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

    public void Set(LevelConfig config)
    {
        levelIdInput.text = config.levelId.ToString();
        foreach (Transform child in enemyListRoot)
            Destroy(child.gameObject);
        
        _enemyRows.Clear();
        foreach (var enemy in config.enemies)
        {
            var row = Instantiate(enemyRowPrefab, enemyListRoot);
            row.Set(enemy);
            _enemyRows.Add(row);
        }
        
        addEnemyButton.onClick.RemoveAllListeners();
        addEnemyButton.onClick.AddListener(AddEnemy);
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