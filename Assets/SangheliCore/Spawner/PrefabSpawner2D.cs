using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PrefabSpawner2D : MonoBehaviour, IPrefabSpawner
{
    [SerializeField] private GameObject prefabToSpawn;
    [SerializeField] private int count = 10;
    [SerializeField] private float minDistance = 1.5f;
    [SerializeField] private Collider2D spawnArea;

    private readonly List<Vector2> _spawnedPositions = new();

    private Transform _parentTf;
    
    public void Init(Transform parentTf)
    {
        _parentTf = parentTf;
        SpawnPrefabs();
    }

    private void SpawnPrefabs()
    {
        if (spawnArea is not BoxCollider2D box)
        {
            Debug.LogError("Требуется BoxCollider2D.");
            return;
        }

        Bounds bounds = box.bounds;

        int attempts = 0;
        int maxAttempts = count * 20;

        while (_spawnedPositions.Count < count && attempts < maxAttempts)
        {
            attempts++;

            var randomPosition = new Vector2
            (
                Random.Range(bounds.min.x, bounds.max.x),
                Random.Range(bounds.min.y, bounds.max.y)
            );

            if (!IsPositionValid(randomPosition)) continue;

            _spawnedPositions.Add(randomPosition);
            Instantiate(prefabToSpawn, randomPosition, Quaternion.identity,_parentTf);
        }

        if (_spawnedPositions.Count < count)
            Debug.LogWarning("Не удалось заспавнить все объекты без перекрытия.");
    }

    private bool IsPositionValid(Vector2 newPos) => 
        !_spawnedPositions.Any(pos => Vector2.Distance(newPos, pos) < minDistance) && spawnArea.OverlapPoint(newPos);
}