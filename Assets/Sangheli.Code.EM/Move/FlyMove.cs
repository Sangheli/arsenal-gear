using UnityEngine;

public class FlyMove : AUnitMove
{
    public override void DoMove(float deltaTime)
    {
        var transform1 = Transform;
        
        var delta = Direction * Speed * deltaTime;
        var position = transform1.position;
        
        position += new Vector3(delta.x, delta.y, 0);
        transform1.position = position;

        if (IsBoss)
        {
            Bounced = RandomCursorInput.MoveBounce3D(position, ref Direction);
        }
        else
        {
            var screenPos = MainCamera.WorldToScreenPoint(position);
            Bounced = RandomCursorInput.MoveBounce(screenPos, ref Direction, PaddingMove);
            Transform.position = MainCamera.ScreenToWorldPoint(screenPos);
        }
        
        if (Bounced) Direction = Direction.normalized;
    }
    
    public override void DoMove()
    {
        DoMove(Time.deltaTime);
    }
}