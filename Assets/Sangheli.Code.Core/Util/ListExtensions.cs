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
    
    public static T PickRandomSingle<T>(this IList<T> source) =>
        source[UnityEngine.Random.Range(0, source.Count)];

    public static T PickRandomSingle<T>(this IEnumerable<T> source) =>
        source.ToList().PickRandomSingle();

    public static T PickRandomSingle<T>(this IEnumerable<T> source, IList<T> forbidden)
    {
        var allowed = source.Where(o => !forbidden.Contains(o)).ToList();

        if (allowed.Count == 0)
            return default;

        return allowed.PickRandomSingle();
    }

    public static List<T> Shuffle<T>(this IEnumerable<T> list)
    {
        var result = new List<T>(list);
        var n = result.Count;

        while (n > 1)
        {
            n--;
            var k = Rand.Next(n + 1);
            (result[k], result[n]) = (result[n], result[k]);
        }

        return result;
    }
}