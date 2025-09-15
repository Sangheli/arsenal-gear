using System;
using System.Numerics;

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
    
    #region public

    public static string Format(double number, int decimalPlaces = 2, bool useSuffix = true)
    {
        var absNumber = Math.Abs(number);
        switch (absNumber)
        {
            case < 1000:
                return FormatSmallNumber(number, decimalPlaces);
            case >= 1000 when useSuffix:
                return FormatWithSuffix(number, decimalPlaces);
            default:
                return FormatWithExponent(number, decimalPlaces);
        }
    }
    
    public static string Format(BigInteger number, int decimalPlaces = 2, bool useSuffix = true)
    {
        var absNumber = BigInteger.Abs(number);
        var decNumber = (decimal)absNumber;
        switch (decNumber)
        {
            case < 1000:
                return FormatSmallNumber((double) decNumber, decimalPlaces);
            case >= 1000 when useSuffix:
                return FormatWithSuffix(decNumber, decimalPlaces);
            default:
                return FormatWithExponent(decNumber, decimalPlaces);
        }
    }
    
    public static string Format(int number, int decimalPlaces = 2, bool useSuffix = true) => Format((double)number, decimalPlaces, useSuffix);
    public static string Format(long number, int decimalPlaces = 2, bool useSuffix = true) => Format((double)number, decimalPlaces, useSuffix);

    #endregion

    #region private format

    private static string FormatWithSuffix(double number, int decimalPlaces)
    {
        var absNumber = Math.Abs(number);
        var exp = (int)Math.Log(absNumber, 1000);
        exp = Math.Min(exp, Suffixes.Length - 1);
        var mantissa = number / Math.Pow(1000, exp);
        var roundedMantissa = RoundValue(mantissa, decimalPlaces);
        return $"{roundedMantissa.ToString($"F{decimalPlaces}")}{Suffixes[exp]}";
    }

    private static string FormatWithSuffix(decimal number, int decimalPlaces)
    {
        var absNumber = Math.Abs(number);
        var exp = 0;
        decimal divisor = 1;
        while (absNumber / divisor >= 1000 && exp < Suffixes.Length - 1)
        {
            divisor *= 1000;
            exp++;
        }
        var mantissa = number / divisor;
        var roundedMantissa = RoundValue(mantissa, decimalPlaces);
        return $"{roundedMantissa.ToString($"F{decimalPlaces}")}{Suffixes[exp]}";
    }

    private static string FormatWithExponent(double number, int decimalPlaces)
    {
        var exponent = 0;
        var reduced = number;
        while (Math.Abs(reduced) >= 1000)
        {
            reduced /= 1000;
            exponent += 3;
        }
        double rounded = RoundValue(reduced, decimalPlaces);
        return $"{rounded.ToString($"F{decimalPlaces}")}xE{exponent}";
    }

    private static string FormatWithExponent(decimal number, int decimalPlaces)
    {
        var exponent = 0;
        var reduced = number;
        while (Math.Abs((double)reduced) >= 1000)
        {
            reduced /= 1000;
            exponent += 3;
        }
        decimal rounded = RoundValue(reduced, decimalPlaces);
        return $"{rounded.ToString($"F{decimalPlaces}")}xE{exponent}";
    }
    
    #endregion

    #region private round

    private static double RoundValue(double value, int decimalPlaces)
    {
        var pow = Math.Pow(10, decimalPlaces);
        return value > 0
            ? Math.Ceiling(value * pow) / pow
            : Math.Floor(value * pow) / pow;
    }

    private static decimal RoundValue(decimal value, int decimalPlaces)
    {
        var pow = (decimal)Math.Pow(10, decimalPlaces);
        return value > 0
            ? Math.Ceiling(value * pow) / pow
            : Math.Floor(value * pow) / pow;
    }
    
    #endregion

    private static string FormatSmallNumber(double number, int decimalPlaces)
    {
        var rounded = RoundValue(number, decimalPlaces);
        return rounded.ToString($"F{decimalPlaces}");
    }
}