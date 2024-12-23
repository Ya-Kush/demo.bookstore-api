using System.Reflection;

namespace App.Endpoints.Services;

public class EndpointContext(IHttpContextAccessor hca, LinkGenerator lg)
{
    public HttpContext HttpContext { get; } = hca.HttpContext ?? throw new NullReferenceException();
    public LinkGenerator LinkGenerator { get; } = lg;

    public string GetLink(Delegate handler, object? values = null)
        => LinkGenerator.GetUriByName(HttpContext, handler.GetMethodInfo().Name, values)
        ?? throw new Exception("Uri Couldn't Be Generated");
}
