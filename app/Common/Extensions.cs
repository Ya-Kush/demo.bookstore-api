using System.Diagnostics.CodeAnalysis;

namespace App.Common;

public static class ObjectExtensions
{
    [return: NotNull]
    public static TR IfNull<T, TR>(this T? val, [NotNull]Func<T?, TR> then, [NotNull]Func<T, TR> otherwise)
        => val is null
        ? then(val)!
        : otherwise(val)!;
}

public static class StringExtensions
{
    public static bool IsNullOrEmpty(this string str) => string.IsNullOrEmpty(str);
    public static bool IsNullOrWhiteSpace(this string str) => string.IsNullOrWhiteSpace(str);
}
