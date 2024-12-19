namespace App.Common;

public static class ResExtensions
{
    public static Res<T> ToRes<T>(this T? val, Func<Error> ifNull) => Res<T>.FailIfNull(val, ifNull);
    public static Res<T> ToRes<T>(this T? val, Error ifNull) => Res<T>.FailIfNull(val, ifNull);
    public static Res<T> ToRes<T>(this T val) => Res<T>.Ok(val);
}
