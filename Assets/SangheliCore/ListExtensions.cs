using System.Collections.Generic;
using System.Linq;

public static class ListExtensions
{
    private static readonly System.Random Rand = new();

    // Берёт num случайных элементов из списка (без повторов)
    // ReSharper disable once MemberCanBePrivate.Global
    public static List<T> PickRandom<T>(this IList<T> source, int num)
    {
        var list = source.ToList(); // делаем копию
        var n = list.Count;

        for (var i = 0; i < num; i++)
        {
            var j = Rand.Next(i, n);
            (list[i], list[j]) = (list[j], list[i]); // swap
        }

        return list.Take(num).ToList();
    }
    
    public static List<T> PickRandom<T>(this IEnumerable<T> source, int num) => 
        source.ToList().PickRandom(num);
}