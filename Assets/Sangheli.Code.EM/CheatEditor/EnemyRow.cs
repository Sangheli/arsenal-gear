using UnityEngine;
using UnityEngine.UI;

public class EnemyRow : MonoBehaviour
{
    public InputField enemyIdInput;
    public InputField spawnChanceInput; // (0..100)
    public InputField spawnRateInput;
    public Button removeButton;

    public void Set(LevelEnemyInfo info)
    {
        enemyIdInput.text = info.enemyId.ToString();
        spawnChanceInput.text = info.spawnChance.ToString("F1"); // (0..100)
        spawnRateInput.text = info.spawnRate.ToString();
        removeButton.onClick.RemoveAllListeners();
        removeButton.onClick.AddListener(RemoveSelf);
    }

    public LevelEnemyInfo GetEnemyInfo()
    {
        var info = new LevelEnemyInfo();
        int.TryParse(enemyIdInput.text, out info.enemyId);
        float.TryParse(spawnChanceInput.text, out info.spawnChance);
        float.TryParse(spawnRateInput.text, out info.spawnRate);
        return info;
    }

    private void RemoveSelf()
    {
        Destroy(gameObject);
    }
} 