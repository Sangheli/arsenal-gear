using UnityEngine;

public class BossView : EnemyView
{
    [Space]
    public float colorMult = 0.8f;

    private Coroutine _bounceRoutineBoss2;
    private Coroutine _bounceShadowRoutineBoss;
    private bool shadowUpdated = false;
    
    public void SetScale(float viewScale)
    {
        render.SetScale(viewScale);
        
        if(shadowUpdated)
            return;

        shadowUpdated = true;
        // render.shadowRenderArr[1].transform.localPosition /= viewScale;
    }

    public void SetColor(Color color, Color borderColor)
    {
        color *= colorMult;
        color.a = 1;
        
        var backColor = color;
        backColor.a = 64 / 255f;
        
        render.OriginalColor = color;
        render.ApplyPropertyView(color);
        render.ApplyPropertyBack(backColor);
        render.ApplyBorderColor(borderColor);
        render.SetOriginalScale(Vector3.one, true);
    }
}