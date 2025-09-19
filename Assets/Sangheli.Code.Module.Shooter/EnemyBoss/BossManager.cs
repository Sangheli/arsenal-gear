using System.Collections;
using UnityEngine;

public class BossManager : MonoBehaviour
{
    public BossController bossPrefab;
    public BossController CurrentBoss { get; private set; }
    private System.Action _onBossDefeated;

    public void SpawnBoss(float hp, Color color, float bossDamage, System.Action onBossDefeated)
    {
        CurrentBoss = G.I.ObjectPool.Get(bossPrefab);
        CurrentBoss.CanAttack = false;
        _onBossDefeated = onBossDefeated;
        G.I.roundManager._spawnPositionManager.GetPosition(G.I.Data.enemyContainer.List[5].PosConfig, out var spawnPosition, out var zPosition);
        CurrentBoss.transform.position = spawnPosition;
        CurrentBoss.gameObject.SetActive(true);
        CurrentBoss.Init(hp, color, bossDamage, OnBossDeath);
        CurrentBoss.StopAllCoroutines();
        CurrentBoss.StartCoroutine(SpawnAndAttack());
    }

    private IEnumerator SpawnAndAttack()
    {
        var centerPos = new Vector3(0, CurrentBoss.CurrentData.PosConfig.maxY, CurrentBoss.CurrentData.PosConfig.customZPosMin);
        yield return SpawnUtils.MoveToCenter(CurrentBoss.transform, centerPos);
        yield return new WaitForSeconds(1);
        if(CurrentBoss!=null)
            CurrentBoss.CanAttack = true;
    }

    private void OnBossDeath()
    {
        G.I.Container.pooledAudioPlayer.PlayWithRandomPitch(G.I.Data.soundContainer.hit);
        G.I.Container.pooledAudioPlayer.PlayWithRandomPitch(G.I.Data.soundContainer.bossDeath);
        _onBossDefeated?.Invoke();
        CurrentBoss = null;
    }

    public void ResetBoss()
    {
        if (CurrentBoss != null)
        {
            G.I.ObjectPool.Return(CurrentBoss);
            CurrentBoss = null;
        }
        
        // Reset boss active flag when boss is reset
        G.I.roundManager.SetBossActive(false);
    }
} 