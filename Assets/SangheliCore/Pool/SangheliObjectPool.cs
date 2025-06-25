using System.Collections.Generic;
using UnityEngine;

public class SangheliObjectPool
{
    private readonly Dictionary<GameObject, Queue<GameObject>> _poolDict = new();
    private readonly Transform _hiddenParent;

    public SangheliObjectPool()
    {
        var hiddenGo = new GameObject("[ObjectPool_Hidden]");
        hiddenGo.SetActive(false);
        _hiddenParent = hiddenGo.transform;
    }

    public T Get<T>(T prefab) where T : Component
    {
        var prefabGo = prefab.gameObject;

        if (!_poolDict.ContainsKey(prefabGo)) _poolDict[prefabGo] = new Queue<GameObject>();
        var pool = _poolDict[prefabGo];

        var result = GetGo(pool, prefabGo);

        _inPool.Remove(result); // убираем из множества

        result.transform.SetParent(null);
        result.gameObject.SetActive(true);
        result.GetComponent<IReturnable>().SetPrefab(prefabGo);
        return result.GetComponent<T>();
    }

    public T Get<T>(T prefab, Vector3 position, Quaternion rotation = default) where T : Component
    {
        var result = Get(prefab);
        if (rotation == default)
            result.transform.position = position;
        else
            result.transform.SetPositionAndRotation(position, rotation);
        return result;
    }

    private static GameObject GetGo(Queue<GameObject> pool, GameObject prefab) => 
        pool.Count > 0 ? pool.Dequeue() : Object.Instantiate(prefab);

    private readonly HashSet<GameObject> _inPool = new();

    public void Return(IReturnable obj1)
    {
        if(obj1 == null) return;
        var obj = obj1.GameObject;
        var prefab = obj1.Prefab;
        
        if (_inPool.Contains(obj)) return; // уже в пуле

        if (obj.TryGetComponent(out IResetable resetable)) resetable.ResetItem();
        obj.transform.SetParent(_hiddenParent);
        obj.SetActive(false);

        if (!_poolDict.TryGetValue(prefab, out var pool))
        {
            pool = new Queue<GameObject>();
            _poolDict[prefab] = pool;
        }

        pool.Enqueue(obj);
        _inPool.Add(obj);
    }
}