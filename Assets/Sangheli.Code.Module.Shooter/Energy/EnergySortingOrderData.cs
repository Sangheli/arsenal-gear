public class EnergySortingOrderData
{
    public int view;
    public int shadow;
    public int Max;
    public int Start;

    public void Iterate()
    {
        view += 2;
        if (view > Max)
            view = Start;
        shadow = view - 1;
    }
} 