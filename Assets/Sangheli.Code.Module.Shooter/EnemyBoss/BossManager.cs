using System.Collections;
using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class BossManager : MonoBehaviour
{
    public BossController bossPrefab;
    public BossController CurrentBoss { get; private set; }
    private System.Action _onBossDefeated;
    private CancellationTokenSource _spawnCancellation;

    public void SpawnBoss(float hp, Color color, float bossDamage, System.Action onBossDefeated)
    {
        CancelSpawn();
        CurrentBoss = G.I.ObjectPool.Get(bossPrefab);
        CurrentBoss.CanAttack = false;
        _onBossDefeated = onBossDefeated;
        G.I.roundManager.SpawnPositionManager.GetPosition(G.I.Data.enemyContainer.List[5].PosConfig, out var spawnPosition, out var zPosition);
        CurrentBoss.transform.position = spawnPosition;
        CurrentBoss.gameObject.SetActive(true);
        CurrentBoss.Init(hp, color, bossDamage, OnBossDeath);
        _spawnCancellation = new CancellationTokenSource();
        _ = SpawnAndAttack(CurrentBoss, _spawnCancellation.Token);
    }

    private async Task SpawnAndAttack(BossController boss, CancellationToken cancellationToken)
    {
        var centerPos = new Vector3(0, boss.CurrentData.PosConfig.maxY, boss.CurrentData.PosConfig.customZPosMin);
        await SpawnUtils.MoveToCenter(boss.transform, centerPos, cancellationToken: cancellationToken);
        await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
        if(CurrentBoss == boss && boss != null)
            boss.CanAttack = true;
    }

    private void OnBossDeath()
    {
        CancelSpawn();
        G.I.Container.pooledAudioPlayer.PlayWithRandomPitch(G.I.Data.soundContainer.hit);
        G.I.Container.pooledAudioPlayer.PlayWithRandomPitch(G.I.Data.soundContainer.bossDeath);
        _onBossDefeated?.Invoke();
        CurrentBoss = null;
    }

    public void ResetBoss()
    {
        CancelSpawn();
        if (CurrentBoss != null)
        {
            G.I.ObjectPool.Return(CurrentBoss);
            CurrentBoss = null;
        }
        
        // Reset boss active flag when boss is reset
        G.I.roundManager.SetBossActive(false);
    }

    private void CancelSpawn()
    {
        if (_spawnCancellation == null)
            return;

        _spawnCancellation.Cancel();
        _spawnCancellation.Dispose();
        _spawnCancellation = null;
    }

    private void OnDestroy()
    {
        CancelSpawn();
    }
}
