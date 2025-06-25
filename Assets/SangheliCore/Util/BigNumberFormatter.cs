using System;
using System.Numerics;

public static class BigNumberFormatter
{
    public static string FormatBigNumber(BigInteger number)
    {
        var absNumber = BigInteger.Abs(number);
        if (absNumber < 1000) return number.ToString();

        int exponent = 0;
        decimal reduced = (decimal)number;

        while (BigInteger.Abs(new BigInteger(reduced)) >= 1000)
        {
            reduced /= 1000;
            exponent += 3;
        }

        return $"{Math.Round(reduced, 2)}xE{exponent}";
    }
}