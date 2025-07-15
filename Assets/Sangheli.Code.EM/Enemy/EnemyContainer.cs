using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Sangheli/EnemyContainer")]
public class EnemyContainer : ScriptableObject
{
    public List<EnemyData> List;

    public void UpdateEnemyIndices()
    {
        for (int i = 0; i < List.Count; i++) List[i].EnemyId = i;
    }
}