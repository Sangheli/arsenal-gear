using UnityEngine;

public class CoinFloorHandler : MonoBehaviour
{
    public Rigidbody2D rb;
    private bool _landed;
    private bool _simulated;
    private float _floorY; // Условный уровень пола
    
    private void OnValidate()
    {
        rb ??= GetComponent<Rigidbody2D>();
    }

    public void Toggle(bool state)
    {
        _simulated = state;
        _landed = false;
        rb.simulated = state;
    }

    private void Update()
    {
        if(!_simulated)
            return;
        
        if (!_landed && transform.position.y <= _floorY)
        {
            Land();
        }
    }

    private void Land()
    {
        _landed = true;

        // Фиксируем монету на линии пола
        var transform1 = transform;
        var pos = transform1.position;
        pos.y = _floorY;
        transform1.position = pos;

        // Отключаем физику
        rb.linearVelocity = Vector2.zero;
        Toggle(false);
    }

    public void SetImpulse(float force, float upwardForce, float floorY)
    {
        _floorY = floorY;
        
        // Случайное направление по X
        var randomX = Random.Range(-1f, 1f);

        // Задаём импульс (разлетается в стороны + вверх)
        var forceDir = new Vector2(randomX * force, upwardForce);
        rb.AddForce(forceDir, ForceMode2D.Impulse);
        Toggle(true);
    }
}