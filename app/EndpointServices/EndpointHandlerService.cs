namespace App.EndpointServices;

public abstract class EndpointHandlerService(EndpointHandlerContext context)
{
    public EndpointHandlerContext Context { get; } = context;

    public string? GetLink(Delegate handler, object? value = null) => Context.GetLink(handler, value);
}
