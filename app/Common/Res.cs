namespace App.Common;

public readonly record struct Res<T>
{
    readonly T? _value;
    readonly Error? _error;

    public T Value => IsOk ? _value! : throw new InvalidOperationException();
    public Error Error => IsFail ? _error! : throw new InvalidOperationException();

    public bool IsOk => _error is null && _value is not null;
    public bool IsFail => _error is not null;


    public Res() => throw new InvalidOperationException("Attempt to create an instance of Res<T> by parameterless constructor");
    Res(T value) => _value = value;
    Res(Error error) => _error = error;

    public static Res<T> Ok(T val) => new(val);
    public static Res<T> Fail(Error err) => new(err);
    public static Res<T> FailIfNull(T? val, Error err) => val is null ? Fail(err) : Ok(val);
    public static Res<T> FailIfNull(T? val, Func<Error> errGetter) => val is null ? Fail(errGetter()) : Ok(val);
    static Res<T> FailIfNull(T? val) => FailIfNull(val, new NullError());


    public T ValueOr(T or) => IsOk ? Value : or;
    public T ValueOr(Func<T> or) => IsOk ? Value : or();

    public Res<T> Tap(Action<T> ok, Action<Error>? fail = default)
    {
        if (IsOk) ok(Value);
        if (fail is not null && IsFail) fail(Error);
        return this;
    }

    public TOut Match<TOut>(Func<T,TOut> ok, Func<Error,TOut> fail) => IsOk
        ? ok(Value)
        : fail(Error);

    public Res<TOut> Bind<TOut>(Func<T,TOut?> ok) => IsOk
        ? Res<TOut>.FailIfNull(ok(Value))
        : Res<TOut>.Fail(Error);

    public Res<TOut> Bind<TOut>(Func<T,TOut?> ok, Func<Error,Res<TOut>> fail) => IsOk
        ? Res<TOut>.FailIfNull(ok(Value))
        : fail(Error);

    public Res<TOut> Map<TOut>(Func<T,Res<TOut>> ok) => IsOk
        ? ok(Value)
        : Res<TOut>.Fail(Error);

    public Res<TOut> Map<TOut>(Func<T,Res<TOut>> ok, Func<Error,Res<TOut>> fail) => IsOk
        ? ok(Value)
        : fail(Error);


    public static implicit operator Res<T>(T? val) => FailIfNull(val);
    public static implicit operator Res<T>(Error err) => Fail(err);
    public static implicit operator Res(Res<T> res) => res.Match(_ => Res.Ok(), Res.Fail);
}

public readonly record struct Res
{
    readonly Error? _error;

    public Error Error => IsFail ? _error! : throw new InvalidOperationException();

    public bool IsOk => _error is null;
    public bool IsFail => _error is not null;


    public Res() => throw new InvalidOperationException("Attempt to create an instance of Res by parameterless constructor");
    Res(Error? error) => _error = error;

    public static Res Ok() => new(default);
    public static Res Fail(Error err) => new(err);
    public static Res FailIfFalse(bool isOk, Error err) => isOk is false ? Fail(err) : Ok();
    public static Res FailIfFalse(bool isOk, Func<Error> errGetter) => isOk is false ? Fail(errGetter()) : Ok();

    public Res Tap(Action ok, Action<Error>? fail = default)
    {
        if (IsOk) ok();
        if (fail is not null && IsFail) fail(Error);
        return this;
    }

    public TOut Match<TOut>(Func<TOut> ok, Func<Error,TOut> fail)
        => IsOk ? ok() : fail(Error);


    public static implicit operator Res(Error err) => Fail(err);
}
