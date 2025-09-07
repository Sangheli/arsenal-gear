public class RunningBarValue
{
    public float HpFill;
    public float WhiteFill;

    public readonly int FillX;
    public readonly int FillY;

    public RunningBarValue(int lower, int upper)
    {
        FillX = lower;
        FillY = upper;
    }

    public void Recalc(float fillValue, float oldFillValue)
    {
        HpFill = fillValue;
        WhiteFill = oldFillValue;
    }
}