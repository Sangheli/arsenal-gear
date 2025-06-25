using System.Collections.Generic;
using UnityEngine;

public class EnemyContainer : MonoBehaviour
{
    public List<EnemyData> List;
    public EnemyData GetRandom() => List[Random.Range(0, List.Count)];

    public void UpdateEnemyIndices()
    {
        for (int i = 0; i < List.Count; i++) List[i].EnemyId = i;
    }
}