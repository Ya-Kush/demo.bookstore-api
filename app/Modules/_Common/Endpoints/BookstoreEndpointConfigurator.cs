using System.Diagnostics.CodeAnalysis;
using App.Data.Models;
using App.Endpoints.Services;

namespace App.Endpoints;

public static class BookstoreEndpointsConfigurator
{
    public static void AddBookstoreEndpointServices(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();
        services.AddScoped<EndpointContext>();
    }

    public static T MapBookstoreEndpoints<T>(this T builder) where T : IEndpointRouteBuilder
    {
        builder.MapAuthors();
        builder.MapBooks();
        builder.MapPublishers();

        return builder;
    }

    static void AddDerivates<T>(this IServiceCollection services, bool withBase)
    {
        var tbase = typeof(T);
        var derivates = tbase.Assembly.ExportedTypes
            .Where(t => t.IsAssignableTo(tbase) && (withBase || t.Equals(tbase) is false));

        foreach (var c in derivates) services.AddScoped(c);
    }
}
