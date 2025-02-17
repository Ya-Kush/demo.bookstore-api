namespace Store.Common;

public record class Error(string Message);

public sealed record class NullError(string Message = "Was Null") : Error(Message);

public sealed record class ExceptionError(Exception Exception) : Error(Exception.Message)
{
    public static implicit operator ExceptionError(Exception exception) => new(exception);
}

public sealed record class ManyErrors(string Message, IEnumerable<Error> Errors) : Error(Message)
{
    public ManyErrors Append(params Error[] newErrors) => Append(newErrors);
    public ManyErrors Append(IEnumerable<Error> newErrors) => new(Message, [..Errors, ..newErrors]);
    public ManyErrors Concat(ManyErrors another) => new(Message, [..Errors, ..another.Errors]);
}


public static class ErrorExtensions
{ 
    public static ManyErrors Concat(this Error error, string message, params Error[] errors)
        => (error, errors) switch
        {
            (ManyErrors many, [ManyErrors another]) => new(message, [..many.Errors, ..another.Errors]),
            (ManyErrors many, _) => new(message, [..many.Errors, ..errors]),
            (_, [ManyErrors many]) => new(message, [error, ..many.Errors]),
            _ => new(message, [error, ..errors])
        };
}