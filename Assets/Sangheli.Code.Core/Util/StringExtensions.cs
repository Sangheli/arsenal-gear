using System;

public static class StringExtensions
{
    /// <summary>
    /// Заменяет все вхождения "\\n" на реальные переводы строки (\n).
    /// </summary>
    public static string ReplaceEscapedNewlines(this string str)
    {
        if (string.IsNullOrEmpty(str))
            return string.Empty;
        return str.Contains("\\n") ? string.Join("\n", str.Split(new[] { "\\n" }, StringSplitOptions.None)) : str;
    }
} 