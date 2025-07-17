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

    public void SpawnBoss(float hp, Color color, float scale, float bossDamage, System.Action onBossDefeated)
    {
        _currentBoss = G.I.ObjectPool.Get(bossPrefab);
        _onBossDefeated = onBossDefeated;
        var spawnPosition = ScreenSpawnHelper.GetRandomScreenPosition(_mainCamera);
        _currentBoss.transform.position = spawnPosition;
        _currentBoss.gameObject.SetActive(true);
        _currentBoss.Init(hp, color, scale, bossDamage, OnBossDeath);
        _currentBoss.StopAllCoroutines();
        _currentBoss.StartCoroutine(SpawnUtils.MoveToCenter(_currentBoss.transform, _mainCamera));
    }

    private void OnBossDeath()
    {
        G.I.Container.pooledAudioPlayer.PlayWithRandomPitch(G.I.Data.soundContainer.bossDeath);
        _onBossDefeated?.Invoke();
        _currentBoss = null;
    }

    public void ResetBoss()
    {
        if (_currentBoss != null)
        {
            G.I.ObjectPool.Return(_currentBoss);
            _currentBoss = null;
        }
    }
} 