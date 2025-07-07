using UnityEngine;

public class BossManager : MonoBehaviour
{
    public BossController bossPrefab;
    private BossController _currentBoss;
    private System.Action _onBossDefeated;
    private Camera _mainCamera;

    private void Awake()
    {
        _mainCamera = Camera.main;
    }

    public void SpawnBoss(float hp, Color color, float scale, System.Action onBossDefeated)
    {
        if (_currentBoss == null)
        {
            _currentBoss = Instantiate(bossPrefab);
        }
        _onBossDefeated = onBossDefeated;
        var spawnPosition = ScreenSpawnHelper.GetRandomScreenPosition(_mainCamera);
        _currentBoss.transform.position = spawnPosition;
        _currentBoss.gameObject.SetActive(true);
        _currentBoss.Init(hp, color, scale, OnBossDeath);
        _currentBoss.StopAllCoroutines();
        _currentBoss.StartCoroutine(SpawnUtils.MoveToCenter(_currentBoss.transform, _mainCamera));
    }

    private void OnBossDeath()
    {
        _onBossDefeated?.Invoke();
    }

    public void ResetBoss()
    {
        if (_currentBoss != null)
            _currentBoss.ResetItem();
    }
} 