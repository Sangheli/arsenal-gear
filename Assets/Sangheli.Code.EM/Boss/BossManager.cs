using System.Collections;
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
        _currentBoss.CanAttack = false;
        _currentBoss.RewindAnim();
        _onBossDefeated = onBossDefeated;
        var spawnPosition = ScreenSpawnHelper.GetRandomScreenPosition();
        _currentBoss.transform.position = spawnPosition;
        _currentBoss.gameObject.SetActive(true);
        _currentBoss.Init(hp, color, scale, bossDamage, OnBossDeath);
        _currentBoss.StopAllCoroutines();
        _currentBoss.StartCoroutine(SpawnAndAttack());
    }

    private IEnumerator SpawnAndAttack()
    {
        yield return SpawnUtils.MoveToCenter(_currentBoss.transform, _mainCamera);
        yield return new WaitForSeconds(1);
        _currentBoss.StartRotAnim();
        _currentBoss.CanAttack = true;
    }

    private void OnBossDeath()
    {
        G.I.Container.pooledAudioPlayer.PlayWithRandomPitch(G.I.Data.soundContainer.hit);
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
        
        // Reset boss active flag when boss is reset
        G.I.roundManager.SetBossActive(false);
    }
} 