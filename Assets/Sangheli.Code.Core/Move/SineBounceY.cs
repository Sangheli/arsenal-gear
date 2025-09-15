using UnityEngine;

public class SineBounceY : MonoBehaviour
{
    public float amplitude = 1f;     // Насколько высоко прыгает
    public float speed = 1f;     // Частота (кол-во колебаний в секунду)
    private float _time;
    private bool _active;

    private void Start()
    {
        _time = 0f;
    }

    void Update()
    {
        if(!_active)
            return;
        
        _time += Time.deltaTime;
        // time = Time.time;
        
        var transform1 = transform;
        Vector3 localPos = transform1.localPosition;
        localPos.y = Mathf.Sin(transform1.parent.position.x + _time*speed) * amplitude;
        transform.localPosition = localPos;
    }

    public void Toggle(bool active)
    {
        _active = active;
    }
}