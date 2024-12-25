using System.Diagnostics.CodeAnalysis;
using App.Data.Models;
using App.Endpoints.Services;

namespace App.Endpoints;

public static class BookstoreEndpointsConfigurator
{
    /// <summary>
    /// Add <see cref="IHttpContextAccessor"/>, scoped <see cref="EndpointContext"/>
    /// and repos for <see cref="Author"/> and for <see cref="Book"/>.
    /// </summary>
    public static void AddBookstoreEndpointServices(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();
        services.AddScoped<EndpointContext>();
        services.AddScoped<AuthorRepo>();
        services.AddScoped<BookRepo>();
    }

    public static RouteGroupBuilder MapBookstoreEndpoints(this IEndpointRouteBuilder routeBuilder, [StringSyntax("Route")] string prefix)
    {
        var group = routeBuilder.MapGroup(prefix).WithTags("Api");

        group.MapAuthors();
        group.MapBooks();
        group.MapPublishers();

        return group;
    }

    static void AddDerivates<T>(this IServiceCollection services, bool withBase)
    {
        var tbase = typeof(T);
        var derivates = tbase.Assembly.ExportedTypes
            .Where(t => t.IsAssignableTo(tbase) && (withBase || t.Equals(tbase) is false));

        foreach (var c in derivates) services.AddScoped(c);
    }
}
