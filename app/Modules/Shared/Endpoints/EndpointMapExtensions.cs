using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace App.Endpoints;

public static class EndpointMapExtensions
{
    public static T MapRouteHandlers<T>(this T routeBuilder, params RouteHandlers[] routeHandlers) where T : IEndpointRouteBuilder
    {
        foreach (var (pattern, handlers) in routeHandlers) routeBuilder.MapHandlers(pattern, handlers);
        return routeBuilder;
    }
    public static T MapHandlers<T>(this T routeBuilder, [StringSyntax("Route")] string pattern, params Delegate[] handlers) where T : IEndpointRouteBuilder
    {
        foreach (var h in handlers) routeBuilder.MapHandler(pattern, h);
        return routeBuilder;
    }
    static RouteHandlerBuilder MapHandler(this IEndpointRouteBuilder routeBuilder, [StringSyntax("Route")] string pattern, Delegate handler)
    {
        var hName = handler.GetMethodInfo().Name;
        var actName = new string([hName[0], ..hName.Skip(1).TakeWhile(x => x is >='a' and <='z')]);
        return actName switch
        {
            "Get" => routeBuilder.MapGet(pattern, handler).WithName(hName),
            "Post" => routeBuilder.MapPost(pattern, handler).WithName(hName),
            "Patch" => routeBuilder.MapPatch(pattern, handler).WithName(hName),
            "Put" => routeBuilder.MapPut(pattern, handler).WithName(hName),
            "Delete" => routeBuilder.MapDelete(pattern, handler).WithName(hName),
            _ => throw new InvalidOperationException()
        };
    }
}
