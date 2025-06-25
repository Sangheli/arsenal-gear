using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace SangheliCode
{
    public class PrefabSpawnerContainer : MonoBehaviour
    {
        [SerializeField] private List<MonoBehaviour> rawList; // для инспектора
        [SerializeField] private Transform transfromToSearchSpawners;
        [SerializeField] private Transform transformToSpawn;

        private List<IPrefabSpawner> list;

        private void OnValidate()
        {
            if (transfromToSearchSpawners == null) 
                return;

            // временный список для сериализации в инспекторе (необходимость из-за невозможности сериализации интерфейсов)
            rawList = new List<MonoBehaviour>();
            list = new List<IPrefabSpawner>();

            foreach (Transform tf in transfromToSearchSpawners)
            {
                if (tf.TryGetComponent<MonoBehaviour>(out var mono) && mono is IPrefabSpawner spawner)
                {
                    list.Add(spawner);
                    rawList.Add(mono);
                }
            }
        }

        private int GetCount() => list.Count(x => x != null);

        public void Init()
        {
            if (list == null || list.Count == 0)
            {
                // если список не был инициализирован через OnValidate
                list = rawList.Where(x => x is IPrefabSpawner).Cast<IPrefabSpawner>().ToList();
            }

            foreach (var x in list)
            {
                x.Init(transformToSpawn);
            }
        }
    }
}
