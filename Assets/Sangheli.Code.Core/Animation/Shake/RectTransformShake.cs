using UnityEngine;

public class RectTransformShake : BaseShake<RectTransform>
{
    protected override Vector2 GetPosition()
    {
        return Target.anchoredPosition;
    }

    protected override void SetPosition(Vector2 pos)
    {
        Target.anchoredPosition = pos;
    }
}
