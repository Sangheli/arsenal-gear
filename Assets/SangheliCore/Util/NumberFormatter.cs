using System;

public static class NumberFormatter
{
    // До "zz" (примерно до 10^78)
    private static readonly string[] Suffixes = 
    {
        "", "k", "m", "b", "t",
        "aa", "ab", "ac", "ad", "ae", "af", "ag", "ah", "ai", "aj", "ak",
        "al", "am", "an", "ao", "ap", "aq", "ar", "as", "at", "au", "av",
        "aw", "ax", "ay", "az", "ba", "bb", "bc", "bd", "be", "bf", "bg",
        "bh", "bi", "bj", "bk", "bl", "bm", "bn", "bo", "bp", "bq", "br",
        "bs", "bt", "bu", "bv", "bw", "bx", "by", "bz", "ca", "cb", "cc",
        "cd", "ce", "cf", "cg", "ch", "ci", "cj", "ck", "cl", "cm", "cn",
        "co", "cp", "cq", "cr", "cs", "ct", "cu", "cv", "cw", "cx", "cy", "cz"
    };

    public static string Format(double number, int decimalPlaces = 2)
    {
        var absNumber = Math.Abs(number);

        if (absNumber < 1000)
            return number.ToString($"F{decimalPlaces}");

        int exp = (int)Math.Log(absNumber, 1000); // количество "троек"
        exp = Math.Min(exp, Suffixes.Length - 1); // чтобы не выйти за пределы массива

        double mantissa = number / Math.Pow(1000, exp);

        return $"{mantissa.ToString($"F{decimalPlaces}")}{Suffixes[exp]}";
    }
}