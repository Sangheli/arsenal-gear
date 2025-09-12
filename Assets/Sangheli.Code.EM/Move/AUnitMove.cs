using UnityEngine;

public abstract class AUnitMove
{
    protected Transform Transform;
    protected float Speed;
    protected Camera MainCamera;
    protected Vector2 Direction;
    protected float PaddingMove;

    protected bool Bounced { get; set; }

    public void Init(Transform transform, float speed, Camera camera, Vector2 direction, float paddingMove)
    {
        Transform = transform;
        Speed = speed;
        MainCamera = camera;
        Direction = direction;
        PaddingMove = paddingMove;
    }
    
    public virtual void DoMove() { }
    public virtual void DoMove(float deltaTime) { }
}