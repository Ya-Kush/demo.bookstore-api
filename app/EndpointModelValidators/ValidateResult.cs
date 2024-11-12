namespace App.EndpointModelValidators;

public readonly struct ValidateResult<T,TError>(T? Value, Func<T?, IEnumerable<TError>> validator)
{
    public readonly IEnumerable<TError> Errors { get; } = validator(Value);
    public readonly bool IsValid => Errors.Any();
}
