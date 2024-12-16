namespace App.Endpoints.Models.Validators;

public readonly struct ValidateResult<T,TError>(T? value, Func<T?, IEnumerable<TError>> validator)
{
    public readonly T? Value { get; } = value;
    public readonly IEnumerable<TError> Errors { get; } = validator(value);
    public readonly bool IsValid => Errors.Any() is false;
    public readonly bool IsInvalid => Errors.Any();
}
