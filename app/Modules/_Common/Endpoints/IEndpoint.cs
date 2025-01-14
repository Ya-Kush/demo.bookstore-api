namespace App.Endpoints;

public interface IEndpoint
{
    Delegate Handler { get; }
}

public interface IGetEndpoint : IEndpoint;
public interface IPostEndpoint : IEndpoint;
public interface IPutEndpoint : IEndpoint;
public interface IPatchEndpoint : IEndpoint;
public interface IDeleteEndpoint : IEndpoint;