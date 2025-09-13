using UnityEngine;

public interface ICollectable
{
    bool IsActive { get; set; }
    void Collect(Transform target, float speed, float delay);
}