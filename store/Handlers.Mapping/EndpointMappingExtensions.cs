using System.Diagnostics.CodeAnalysis;

namespace Store.Handlers.Mapping;

public static class EndpointMappingExtensions
{
    public static RouteMapperExtensions.RouteMapper Map(this IEndpointRouteBuilder routeBuilder, [StringSyntax("Route")]string pattern)
        => new(pattern, routeBuilder);

    public static RouteHandlerBuilder Map<T>(this IEndpointRouteBuilder routeBuilder, [StringSyntax("Route")]string pattern) where T : IHandler, new()
    {
        Type type = typeof(T);
        string name = type.Name;
        IHandler impl = new T();
        Delegate handler = impl.Handler;

        var handlerBuilder = impl switch
        {
            IGetHandler => routeBuilder.MapGet(pattern, handler),
            IPostHandler => routeBuilder.MapPost(pattern, handler),
            IPutHandler => routeBuilder.MapPut(pattern, handler),
            IPatchHandler => routeBuilder.MapPatch(pattern, handler),
            IDeleteHandler => routeBuilder.MapDelete(pattern, handler),
            _ => throw new InvalidOperationException("Must not be another case")
        };
        handlerBuilder.WithName(name);

        return handlerBuilder;
    }
}

public static class RouteMapperExtensions
{
    public readonly record struct RouteMapper(string Pattern, IEndpointRouteBuilder Router);
    public readonly record struct RouteManyMapper(string Pattern, IEndpointRouteBuilder Router);

    public static RouteHandlerBuilder With<T>(this RouteMapper mapper) where T : IHandler, new()
        => mapper.Router.Map<T>(mapper.Pattern);

    public static IEndpointRouteBuilder Complete(this RouteMapper mapper) => mapper.Router;

    public static IEndpointRouteBuilder WithCompleted<T>(this RouteMapper mapper) where T : IHandler, new()
    {
        mapper.Router.Map<T>(mapper.Pattern);
        return mapper.Router;
    }

    public static IEndpointRouteBuilder WithCompleted<T>(this RouteMapper mapper, Action<RouteHandlerBuilder> configure) where T : IHandler, new()
    {
        configure(mapper.Router.Map<T>(mapper.Pattern));
        return mapper.Router;
    }

    public static RouteManyMapper WithMany(this RouteMapper mapper)
        => new(mapper.Pattern, mapper.Router);

    public static RouteManyMapper Add<T>(this RouteManyMapper mapper) where T : IHandler, new()
    {
        mapper.Router.Map<T>(mapper.Pattern);
        return mapper;
    }

    public static RouteManyMapper Add<T>(this RouteManyMapper mapper, Action<RouteHandlerBuilder> configure) where T : IHandler, new()
    {
        configure(mapper.Router.Map<T>(mapper.Pattern));
        return mapper;
    }

    public static IEndpointRouteBuilder Last<T>(this RouteManyMapper mapper) where T : IHandler, new() => Add<T>(mapper).Router;
    public static IEndpointRouteBuilder Last<T>(this RouteManyMapper mapper, Action<RouteHandlerBuilder> configure) where T : IHandler, new() => Add<T>(mapper, configure).Router;
}