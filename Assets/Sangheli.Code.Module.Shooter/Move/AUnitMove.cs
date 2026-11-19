using UnityEngine;

public abstract class AUnitMove
{
    protected Transform Transform;
    protected float Speed;
    protected Camera MainCamera;
    protected Vector3 Direction;
    protected float PaddingMove;
    protected bool PingPong;
    protected System.Action<Vector3> ChangeDirectionCallback;
    protected bool IsBoss;

    protected bool Bounced { get; set; }

    public void Init(Transform transform, float speed, Camera camera, Vector2 direction, float paddingMove, bool pingPong,bool isBoss, System.Action<Vector3> changeDirectionCallback)
    {
        Transform = transform;
        Speed = speed;
        MainCamera = camera;
        Direction = direction;
        PaddingMove = paddingMove;
        PingPong = pingPong;

        if (PingPong)
            Speed *= Random.Range(29f,31f);

        ChangeDirectionCallback = changeDirectionCallback;
        IsBoss = isBoss;
    }
    
    public virtual void DoMove() { }
    public virtual void DoMove(float deltaTime) { }
    public virtual void LateUpdate(){}
}