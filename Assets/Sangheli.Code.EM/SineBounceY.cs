using UnityEngine;

public class SineBounceY : MonoBehaviour
{
    public float amplitude = 1f;     // Насколько высоко прыгает
    public float speed = 1f;     // Частота (кол-во колебаний в секунду)
    private float time;
    private bool _active;

    void Start()
    {
        time = 0f;
    }

    void Update()
    {
        if(!_active)
            return;
        
        time += Time.deltaTime;
        // time = Time.time;
        Vector3 localPos = transform.localPosition;
        localPos.y = Mathf.Sin(transform.parent.position.x + time*speed) * amplitude;
        transform.localPosition = localPos;
    }

    public void Toggle(bool active)
    {
        _active = active;
    }
}