[System.Serializable]
public class PrestigeData
{
    public int prespawnFirstTime = 10; // сколько спавнится врагов в первый запуск престижа
    public int prespawn = 10; // преспавн в остальных случаях
    public float bossHP = 1000;

    public int GetPrespawnCount(bool played) => played ? prespawn : prespawnFirstTime;
}