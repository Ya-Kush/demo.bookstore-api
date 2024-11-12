using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace App.EndpointHandlers;

public static class EndpointsBuilderExtensions
{
    public static RouteHandlerBuilder MapGetNamedAsHandler(this IEndpointRouteBuilder routeBuilder, [StringSyntax("Route")] string pattern, Delegate handler)
        => routeBuilder.MapGet(pattern, handler).WithName(handler.GetMethodInfo().Name);

    public static RouteHandlerBuilder MapPostNamedAsHandler(this IEndpointRouteBuilder routeBuilder, [StringSyntax("Route")] string pattern, Delegate handler)
        => routeBuilder.MapPost(pattern, handler).WithName(handler.GetMethodInfo().Name);

    public static RouteHandlerBuilder MapPutNamedAsHandler(this IEndpointRouteBuilder routeBuilder, [StringSyntax("Route")] string pattern, Delegate handler)
        => routeBuilder.MapPut(pattern, handler).WithName(handler.GetMethodInfo().Name);

    public static RouteHandlerBuilder MapPatchNamedAsHandler(this IEndpointRouteBuilder routeBuilder, [StringSyntax("Route")] string pattern, Delegate handler)
        => routeBuilder.MapPatch(pattern, handler).WithName(handler.GetMethodInfo().Name);

    public static RouteHandlerBuilder MapDeleteNamedAsHandler(this IEndpointRouteBuilder routeBuilder, [StringSyntax("Route")] string pattern, Delegate handler)
        => routeBuilder.MapDelete(pattern, handler).WithName(handler.GetMethodInfo().Name);
}
