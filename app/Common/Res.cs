namespace App.Common;

public interface IRes<T>;

public readonly record struct Ok<T>(T Value) : IRes<T>;
public readonly record struct Created<T>(T Value) : IRes<T>;
public readonly record struct Updated<T>(T Value) : IRes<T>;

public readonly record struct Fail<T>(T? Value) : IRes<T>;
public readonly record struct NotFound<T>(T? Value) : IRes<T>;

public readonly record struct Error<T>(T? Value, IEnumerable<string> Errors) : IRes<T>;
public readonly record struct Error<T,TE>(T? Value, IEnumerable<TE> Errors) : IRes<T>;

public static class Res
{
    public static IRes<T> Ok<T>(T value) => new Ok<T>(value);
    public static IRes<T> Created<T>(T value) => new Created<T>(value);
    public static IRes<T> Updated<T>(T value) => new Updated<T>(value);

    public static IRes<T> Fail<T>(T? value) => new Fail<T>(value);
    public static IRes<T> NotFound<T>(T? value) => new NotFound<T>(value);

    public static IRes<T> Error<T>(IEnumerable<string> errors) => new Error<T,string>(default, errors);
    public static IRes<T> Error<T>(T? value, IEnumerable<string> errors) => new Error<T,string>(value, errors);
    public static IRes<T> Error<T,TE>(IEnumerable<TE> errors) => new Error<T,TE>(default, errors);
    public static IRes<T> Error<T,TE>(T? value, IEnumerable<TE> errors) => new Error<T,TE>(value, errors);
}

public static class ResExtensions
{
    static TR? If<T, TT, TR>(this IRes<T> res, Func<TT, TR> then, Func<IRes<T>, TR?>? otherwise = null)
        where TT : IRes<T>
        => res is TT target
        ? then(target)
        : otherwise is null ? default : otherwise(res);

    public static TR? IfOk<T,TR>(this IRes<T> res, Func<Ok<T>, TR> then, Func<IRes<T>, TR?>? otherwise = null) => If(res, then, otherwise);
    public static TR? IfCreated<T,TR>(this IRes<T> res, Func<Created<T>, TR> then, Func<IRes<T>, TR?>? otherwise = null) => If(res, then, otherwise);
    public static TR? IfUpdated<T,TR>(this IRes<T> res, Func<Updated<T>, TR> then, Func<IRes<T>, TR?>? otherwise = null) => If(res, then, otherwise);

    public static TR? IfFail<T, TR>(this IRes<T> res, Func<Fail<T>, TR> then, Func<IRes<T>, TR?>? otherwise = null) => If(res, then, otherwise);
    public static TR? IfNotFound<T,TR>(this IRes<T> res, Func<NotFound<T>, TR> then, Func<IRes<T>, TR?>? otherwise = null) => If(res, then, otherwise);

    public static TR? IfError<T,TR>(this IRes<T> res, Func<Error<T>, TR> then, Func<IRes<T>, TR?>? otherwise = null) => If(res, then, otherwise);
    public static TR? IfError<T,TE,TR>(this IRes<T> res, Func<Error<T,TE>, TR> then, Func<IRes<T>, TR?>? otherwise = null) => If(res, then, otherwise);
}
