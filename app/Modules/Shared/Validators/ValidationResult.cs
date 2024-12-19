using App.Common;

namespace App.Endpoints.Models.Validators;

public readonly struct ValidationResult<T,TError>
{
    public readonly T? Value { get; }
    public readonly ValidationError<TError> Error;

    public ValidationResult(T? value, Func<T?, IEnumerable<TError>> validator)
    {
        Value = value;
        var errors = validator(value);
        Error = errors.Any()
        ? new(errors)
        : ValidationError<TError>.Empty;
    }

    public readonly bool IsValid => Error.IsEmpty;
    public readonly bool IsInvalid => IsValid is false;
}

public sealed record class ValidationError<T>(IEnumerable<T> Errors, string Message = "Validation Error") : Error(Message)
{
    public readonly static ValidationError<T> Empty = new([]);
    public bool IsEmpty => this == Empty;
}
