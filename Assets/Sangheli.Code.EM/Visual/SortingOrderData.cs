public class SortingOrderData
{
    public int ramka;
    public int view;
    public int shadow;
    
    public int Max;
    public int Start;

    public void Iterrate()
    {
        ramka+=3;
        if (ramka > Max)
            ramka = Start;

        view = ramka - 1;
        shadow = ramka - 2;
    }
}