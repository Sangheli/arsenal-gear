using UnityEngine;

public class LinearMove : AUnitMove
{
    public override void DoMove(float deltaTime)
    {
        Transform.Translate(Direction * (deltaTime * Speed));
    }
    
    public override void DoMove()
    {
        DoMove(Time.deltaTime);
    }
}