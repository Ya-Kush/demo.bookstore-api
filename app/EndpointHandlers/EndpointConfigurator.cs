using System.Diagnostics.CodeAnalysis;
using App.EndpointServices;

namespace App.EndpointHandlers;

public static class EndpointsConfigurator
{
    /// <summary>
    /// Add <see cref="IHttpContextAccessor"/> and scoped <see cref="EndpointHandlerContext"/>'s derivates
    /// </summary>
    public static void AddEndpointHandlers(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();
        services.AddDerivates<EndpointHandlerContext>(withBase: true);
        services.AddDerivates<EndpointHandlerService>(withBase: false);
    }

    static void AddDerivates<T>(this IServiceCollection services, bool withBase)
    {
        var tbase = typeof(T);
        var derivates = tbase.Assembly.ExportedTypes
            .Where(t => t.IsAssignableTo(tbase) && (withBase || t.Equals(tbase) is false));

        foreach (var c in derivates) services.AddScoped(c);
    }

    public static RouteGroupBuilder MapEndpointHandlers(this IEndpointRouteBuilder routeBuilder, [StringSyntax("Route")] string prefix)
    {
        var group = EndpointRouteBuilderExtensions.MapGroup(routeBuilder, prefix).WithName("Api");

        group.MapBooks();
        group.MapAuthors();

        return group;
    }
}
