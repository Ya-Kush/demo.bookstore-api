namespace App.Handlers;

public interface IHandler
{
    Delegate Handler { get; }
}

public interface IGetHandler : IHandler;
public interface IPostHandler : IHandler;
public interface IPutHandler : IHandler;
public interface IPatchHandler : IHandler;
public interface IDeleteHandler : IHandler;