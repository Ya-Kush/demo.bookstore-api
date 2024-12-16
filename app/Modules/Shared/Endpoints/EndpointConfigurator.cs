using System.Diagnostics.CodeAnalysis;
using App.Data.Models;
using App.Endpoints.Services;

namespace App.Endpoints;

public static class EndpointsConfigurator
{
    /// <summary>
    /// Add <see cref="IHttpContextAccessor"/>, scoped <see cref="EndpointContext"/>
    /// and repos for <see cref="Author"/> and for <see cref="Book"/>.
    /// </summary>
    public static void AddEndpointServices(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();
        services.AddScoped<EndpointContext>();
        services.AddScoped<AuthorRepo>();
        services.AddScoped<BookRepo>();
    }

    static void AddDerivates<T>(this IServiceCollection services, bool withBase)
    {
        var tbase = typeof(T);
        var derivates = tbase.Assembly.ExportedTypes
            .Where(t => t.IsAssignableTo(tbase) && (withBase || t.Equals(tbase) is false));

        foreach (var c in derivates) services.AddScoped(c);
    }

    public static RouteGroupBuilder MapEndpoints(this IEndpointRouteBuilder routeBuilder, [StringSyntax("Route")] string prefix)
    {
        var group = EndpointRouteBuilderExtensions.MapGroup(routeBuilder, prefix).WithName("Api");

        group.MapBooks();
        group.MapAuthors();

        return group;
    }
}
