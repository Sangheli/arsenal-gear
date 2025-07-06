using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class LevelRow : MonoBehaviour
{
    public InputField levelIdInput;
    public Transform enemyListRoot;
    public GameObject enemyRowPrefab;
    public Button addEnemyButton;

    private List<EnemyRow> enemyRows = new List<EnemyRow>();
    private LevelConfig _levelConfig;

    public void Set(LevelConfig config)
    {
        _levelConfig = config;
        levelIdInput.text = config.levelId.ToString();
        foreach (Transform child in enemyListRoot)
            Destroy(child.gameObject);
        enemyRows.Clear();
        foreach (var enemy in config.enemies)
        {
            var rowObj = Instantiate(enemyRowPrefab, enemyListRoot);
            var row = rowObj.GetComponent<EnemyRow>();
            row.Set(enemy);
            enemyRows.Add(row);
        }
        addEnemyButton.onClick.RemoveAllListeners();
        addEnemyButton.onClick.AddListener(AddEnemy);
    }

    public LevelConfig GetLevelConfig()
    {
        var config = new LevelConfig();
        int.TryParse(levelIdInput.text, out config.levelId);
        config.enemies = new List<LevelEnemyInfo>();
        foreach (var row in enemyRows)
            config.enemies.Add(row.GetEnemyInfo());
        return config;
    }

    private void AddEnemy()
    {
        var rowObj = Instantiate(enemyRowPrefab, enemyListRoot);
        var row = rowObj.GetComponent<EnemyRow>();
        row.Set(new LevelEnemyInfo());
        enemyRows.Add(row);
    }
} 