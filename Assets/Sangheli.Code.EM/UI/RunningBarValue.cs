public class RunningBarValue
{
    public float ClipplingLeftFill = 0;
    public float ClipplingRightFill;

    public float ClipplingLeftWhite;
    public float ClipplingRightWhite;

    public float ClipplingLeftBack;
    public float ClipplingRightBack = 0;

    public readonly int ClipUvLeft;
    public readonly int ClipUvRight;

    public RunningBarValue(int lower, int upper)
    {
        ClipUvLeft = lower;
        ClipUvRight = upper;
    }

    public void Recalc(float fillValue, float oldFillValue)
    {
        ClipplingRightFill = 1 - fillValue;
        
        ClipplingLeftWhite = fillValue;
        ClipplingRightWhite = 1 - oldFillValue;
        
        ClipplingLeftBack = oldFillValue;
    }
}