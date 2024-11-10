namespace App.Endpoints;

public static partial class MapEndpointsExtensions
{
    public static IEndpointRouteBuilder MapBookstoreApi(this IEndpointRouteBuilder routeBuilder, string prefix)
        => routeBuilder.MapGroup(prefix)
            .MapBooks()
            .MapAuthors();
}