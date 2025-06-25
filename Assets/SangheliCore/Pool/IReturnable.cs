using UnityEngine;

public interface IReturnable
{
    GameObject GameObject { get; }
    GameObject Prefab { get; set; }
    void SetPrefab(GameObject prefab) => Prefab = prefab;
}