using System.Collections;
using UnityEngine;

public class BossManager : MonoBehaviour
{
    public BossController bossPrefab;
    private BossController _currentBoss;
    private System.Action _onBossDefeated;

    public void SpawnBoss(float hp, Color color, float bossDamage, System.Action onBossDefeated)
    {
        _currentBoss = G.I.ObjectPool.Get(bossPrefab);
        _currentBoss.CanAttack = false;
        _onBossDefeated = onBossDefeated;
        G.I.roundManager._spawnPositionManager.GetPosition(G.I.Data.enemyContainer.List[5].PosConfig, out var spawnPosition, out var zPosition);
        _currentBoss.transform.position = spawnPosition;
        _currentBoss.gameObject.SetActive(true);
        _currentBoss.Init(hp, color, bossDamage, OnBossDeath);
        _currentBoss.StopAllCoroutines();
        _currentBoss.StartCoroutine(SpawnAndAttack());
    }

    private IEnumerator SpawnAndAttack()
    {
        var centerPos = new Vector3(0, _currentBoss.CurrentData.PosConfig.maxY, _currentBoss.CurrentData.PosConfig.customZPosMin);
        yield return SpawnUtils.MoveToCenter(_currentBoss.transform, centerPos);
        yield return new WaitForSeconds(1);
        if(_currentBoss!=null)
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