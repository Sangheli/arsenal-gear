using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Text;
using System.Collections.Generic;

public class PrestigeGainedView : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private GameObject view;
    [SerializeField] private Text descriptionText;
    [SerializeField] private Button continueButton;
    
    private void Awake()
    {
        continueButton.onClick.AddListener(OnContinue);
    }
    
    public void Show()
    {
        var currentPrestige = G.I.SaveProvider.Get().prestige;
        var upgradesText = GetNewlyUnlockedUpgradesText(currentPrestige);
        descriptionText.text = $"You've reached prestige level {currentPrestige}!\nNew enemies and challenges await!\n\n{upgradesText}";
        Toggle(true);
    }
    
    private void OnContinue()
    {
        Toggle(false);
        G.I.endRoundView.Show();
    }
    
    public void Toggle(bool state)
    {
        view.SetActive(state);
    }
    
    private string GetNewlyUnlockedUpgradesText(int prestige)
    {
        var sb = new StringBuilder();
        
        // Новые враги
        var newEnemies = G.I.enemyContainer.List
            .Where(e => e.minPrestigeToSpawn == prestige)
            .ToList();
        if (newEnemies.Count > 0)
        {
            sb.AppendLine("New enemies:");
            foreach (var enemy in newEnemies)
                sb.AppendLine($" - {enemy.DisplayName}");
            sb.AppendLine();
        }
        
        // Player upgrades
        var newPlayerUpgrades = ParamData.Params
            .Where(p => p.Value.MinPrestige == prestige)
            .Select(p => p.Value.DisplayName)
            .ToList();
        if (newPlayerUpgrades.Count > 0)
        {
            sb.AppendLine("New player upgrades:");
            foreach (var name in newPlayerUpgrades)
                sb.AppendLine($" - {name}");
            sb.AppendLine();
        }
        
        // Enemy upgrades только для новых врагов
        var newEnemyUpgrades = new List<string>();
        foreach (var enemy in newEnemies)
        {
            var enemyNewUpgrades = enemy.UpgradeInfos
                .Where(u => u.MinPrestige == prestige)
                .Select(u => $"[{enemy.DisplayName}] {u.DisplayName}");
            newEnemyUpgrades.AddRange(enemyNewUpgrades);
        }
        if (newEnemyUpgrades.Count > 0)
        {
            sb.AppendLine("New enemy upgrades:");
            foreach (var name in newEnemyUpgrades)
                sb.AppendLine($" - {name}");
            sb.AppendLine();
        }
        
        if (sb.Length == 0)
            sb.AppendLine("No new upgrades or enemies unlocked at this prestige.");
        
        return sb.ToString();
    }
} 