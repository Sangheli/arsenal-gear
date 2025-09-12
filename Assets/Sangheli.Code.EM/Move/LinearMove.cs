using UnityEngine;

public class LinearMove : AUnitMove
{
    public override void DoMove(float deltaTime)
    {
        Transform.Translate(Direction * (deltaTime * Speed));
    }

    public override void LateUpdate()
    {
        if (!PingPong) return;

        var position = Transform.position;
            
        float border = G.I.BoundX - 0.5f;
        float posX = position.x;

        // Достигли правой границы
        if (posX >= border && Direction.x > 0)
        {
            Direction = new Vector2(-1, Direction.y);
            position.x = border;
            Transform.position = position;
            ChangeDirectionCallback.Invoke(Direction);
        }
        // Достигли левой границы
        else if (posX <= -border && Direction.x < 0)
        {
            Direction = new Vector2(1, Direction.y);
            position.x = -border;
            Transform.position = position;
            ChangeDirectionCallback.Invoke(Direction);
        }
    }
    
    public override void DoMove()
    {
        DoMove(Time.deltaTime);
    }
}