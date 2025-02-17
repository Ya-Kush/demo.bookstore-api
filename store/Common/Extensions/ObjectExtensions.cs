using System.Diagnostics.CodeAnalysis;

namespace Store.Common.Extensions;

public static class ObjectExtensions
{
    [return: NotNull]
    public static R IfNull<T, R>(this T? val, [NotNull]Func<T?, R> then, [NotNull]Func<T, R> otherwise)
        => val is null
        ? then(val)!
        : otherwise(val)!;

    [return: NotNull]
    public static T IfNull<T>(this T? val, [NotNull]Func<T> then)
        => val is null ? then()! : val;

    [return: NotNull]
    public static T IfNull<T>(this T? val, [NotNull] T then)
    {
        if (then is null) throw new NullReferenceException();
        return val ?? then;
    }


    public static void IfNotNull<T>(this T? val, Action<T> then)
    {
        if (val is not null) then(val);
    }

    public static R? IfNotNull<T,R>(this T? val, Func<T,R> then)
        => val is not null ? then(val) : default;

    public static R IfNotNull<T,R>(this T? val, Func<T,R> then, Func<T?,R> otherwise)
        => val is not null ? then(val) : otherwise(val);
}
