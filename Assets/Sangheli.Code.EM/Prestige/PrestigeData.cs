using UnityEngine;

[System.Serializable]
public class PrestigeData
{
    public int Index { get; set; } // уровень престижа
    public int prespawnFirstTime = 10; // сколько спавнится врагов в первый запуск престижа
    public int prespawn = 10; // преспавн в остальных случаях
    public Color color = UnityEngine.Color.white; // цвет подложки престижа
    public float bossHP = 1000;
    public float bossTime = 60; // время до появления босса

    public int GetPrespawnCount(bool played) => played ? prespawn : prespawnFirstTime;
}