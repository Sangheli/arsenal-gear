using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UpgradeRowView : MonoBehaviour
{
    [SerializeField] private Transform upgradeContainer;
    [SerializeField] private UpgradeStateView upgradeViewPrefab;
    [SerializeField] private EnemyUpgradeStateView enemyUpgradeViewPrefab;
    
    public List<BaseUpgradeStateView> views = new();
    public int rowIndex { get; private set; }
    
    public void Initialize(int row)
    {
        rowIndex = row;
    }
    
    public void SpawnUpgradeView(string paramKey, System.Action refreshMoneyCallback = null)
    {
        if (upgradeViewPrefab == null || upgradeContainer == null)
        {
            Debug.LogError("UpgradeRowView: upgradeViewPrefab or upgradeContainer is not assigned!");
            return;
        }
        
        var upgradeObj = Instantiate(upgradeViewPrefab, upgradeContainer);
        var upgradeView = upgradeObj.GetComponent<UpgradeStateView>();
        
        if (upgradeView != null)
        {
            upgradeView.Init(paramKey, refreshMoneyCallback);
            views.Add(upgradeView);
        }
        else
        {
            Debug.LogError("UpgradeRowView: upgradeViewPrefab doesn't have UpgradeStateView component!");
        }
    }
    
    public void SpawnEnemyUpgradeView(int enemyId, UpgradeType upgradeType, System.Action refreshMoneyCallback = null)
    {
        if (enemyUpgradeViewPrefab == null || upgradeContainer == null)
        {
            Debug.LogError("UpgradeRowView: enemyUpgradeViewPrefab or upgradeContainer is not assigned!");
            return;
        }
        
        var upgradeObj = Instantiate(enemyUpgradeViewPrefab, upgradeContainer);
        var enemyUpgradeView = upgradeObj.GetComponent<EnemyUpgradeStateView>();
        
        if (enemyUpgradeView != null)
        {
            enemyUpgradeView.Init(enemyId, upgradeType, refreshMoneyCallback);
            views.Add(enemyUpgradeView);
        }
        else
        {
            Debug.LogError("UpgradeRowView: enemyUpgradeViewPrefab doesn't have EnemyUpgradeStateView component!");
        }
    }
}