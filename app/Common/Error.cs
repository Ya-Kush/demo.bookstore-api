namespace App.Common;

public record class Error(string Message);

public sealed record class ManyErrors(IEnumerable<Error> Errors, string Message = "Many Errors") : Error(Message)
{
    public ManyErrors Append(params Error[] newErrors) => new([..Errors, ..newErrors], Message);
    public ManyErrors Append(IEnumerable<Error> newErrors) => new([..Errors, ..newErrors], Message);
}

public sealed record class NullError(string Message = "Was Null") : Error(Message);

public sealed record class ExceptionError(Exception Exception) : Error(Exception.Message)
{
    public static implicit operator ExceptionError(Exception exception) => new(exception);
}


public static class ErrorExtensions
{ 
    public static ManyErrors Concat(this Error err, params Error[] errors)
        => (err, errors) switch
        {
            (ManyErrors many, [ManyErrors manyErrors]) => many.Append(manyErrors.Errors),
            (ManyErrors many, _) => many.Append(errors),
            (_, [ManyErrors manyErrors]) => manyErrors.Append(err),
            _ => new ManyErrors([err, ..errors])
        };
}
