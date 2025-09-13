public class SortingOrderData
{
    public int ramka;
    public int view;
    public int back;
    public int shadow;
    
    public int Max;
    public int Start;

    public void Iterrate()
    {
        ramka+=4;
        if (ramka > Max)
            ramka = Start;

        view = ramka - 1;
        back = ramka - 2;
        shadow = ramka - 3;
    }
}