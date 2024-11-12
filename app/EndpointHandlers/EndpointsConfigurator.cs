namespace App.EndpointHandlers;

public static class BookstoreEndpointsConfigurator
{
    /// <summary>
    /// Add <see cref="HttpContextAccessor"/> and scoped <see cref="EndpointHandlerContext"/>
    /// </summary>
    public static void AddEndpointHandlerContext(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();
        services.AddScoped<EndpointHandlerContext>();
    }

    public static RouteGroupBuilder MapBookstoreEndpointHandlers(this IEndpointRouteBuilder routeBuilder, string prefix)
    {
        var group = routeBuilder.MapGroup(prefix).WithName("Api");

        group.MapHome();
        group.MapBooks();
        group.MapAuthors();

        return group;
    }

    static void MapHome(this IEndpointRouteBuilder routeBuilder)
    {
        routeBuilder.MapGet("/", (HttpContext hc, LinkGenerator lg) => new
        {
            links = new
            {
                self = lg.GetUriByName(hc, "Api"),
                books = lg.GetUriByName(hc, "GetBooks"),
                authors = lg.GetUriByName(hc, "GetAuthors"),
            }
        });
    }
}
