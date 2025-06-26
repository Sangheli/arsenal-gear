using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Text;

public class PrestigeGainedView : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private GameObject view;
    [SerializeField] private Text descriptionText;
    [SerializeField] private Button continueButton;
    
    [Header("Animation Settings")]
    [SerializeField] private float showDuration = 2f;
    [SerializeField] private float bounceScale = 1.2f;
    [SerializeField] private float bounceDuration = 0.5f;
    
    private void Awake()
    {
        if (continueButton != null)
            continueButton.onClick.AddListener(OnContinue);
        
        Toggle(false);
    }
    
    public void Show()
    {
        StartCoroutine(ShowPrestigeGained());
    }
    
    private IEnumerator ShowPrestigeGained()
    {
        // Получаем текущий престиж
        var currentPrestige = G.I.SaveProvider.Get().prestige;

        // Формируем текст новых апгрейдов
        string upgradesText = GetNewlyUnlockedUpgradesText(currentPrestige);
        
        // Обновляем UI
        descriptionText.text = $"You've reached prestige level {currentPrestige}!\nNew enemies and challenges await!\n\n{upgradesText}";
        
        // Показываем окно
        Toggle(true);
        
        // Анимация появления
        if (view != null)
        {
            view.transform.localScale = Vector3.zero;
            yield return StartCoroutine(GameAnimation.Bounce(view.transform, Vector3.one, bounceScale, bounceDuration));
        }
        
        // Воспроизводим звук
        G.I.Container.pooledAudioPlayer.PlayWithRandomPitch(G.I.Data.soundContainer.checkpoint);
        
        // Ждем показывания
        yield return new WaitForSeconds(showDuration);
        
        // Активируем кнопку продолжения
        if (continueButton != null)
            continueButton.interactable = true;
    }
    
    private void OnContinue()
    {
        Toggle(false);
        G.I.endRoundView.Show();
    }
    
    public void Toggle(bool state)
    {
        view.SetActive(state);
        continueButton.interactable = false;
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
        var newEnemyUpgrades = new System.Collections.Generic.List<string>();
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