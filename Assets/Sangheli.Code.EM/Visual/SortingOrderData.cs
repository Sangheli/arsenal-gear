public class SortingOrderData
{
    public int Current;
    public int Base;
    public int Max;
    public int Start;

    public int GetNext()
    {
        var retValue = Current;
        Current++;
        if (Current > Max)
            Current = Start;
        return retValue;
    }
}