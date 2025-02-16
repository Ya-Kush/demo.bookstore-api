using System.Diagnostics.CodeAnalysis;

namespace Auth.Extensions;

public static class Mapping
{
    public static RouteGroupBuilder MapGroup(this IEndpointRouteBuilder routeBuilder, [StringSyntax("Route")]string prefix, Action<IEndpointRouteBuilder> internalMapper)
    {
        var group = routeBuilder.MapGroup(prefix);
        internalMapper(group);
        return group;
    }
}