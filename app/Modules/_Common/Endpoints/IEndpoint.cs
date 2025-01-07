using System.Diagnostics.CodeAnalysis;

namespace App.Endpoints;

public interface IEndpoint
{
    Delegate Handler { get; }
}

public interface IGetEndpoint : IEndpoint;
public interface IPostEndpoint : IEndpoint;
public interface IPutEndpoint : IEndpoint;
public interface IPatchEndpoint : IEndpoint;
public interface IDeleteEndpoint : IEndpoint;

public static class EndpointMappingExtensions
{
    public record struct RouteEndpointBinder(string Pattern, IEndpointRouteBuilder Builder)
    {
        public readonly RouteHandlerBuilder With<T>() where T : IEndpoint, new() => Builder.Map<T>(Pattern);

        public readonly RouteGroupBuilder With<T1,T2>()
            where T1 : IEndpoint, new()
            where T2 : IEndpoint, new()
        {
            var group = Builder.MapGroup(Pattern);
            group.Map<T1>("");
            group.Map<T2>("");
            return group;
        }

        public readonly RouteGroupBuilder With<T1,T2,T3>()
            where T1 : IEndpoint, new()
            where T2 : IEndpoint, new()
            where T3 : IEndpoint, new()
        {
            var group = Builder.MapGroup(Pattern);
            group.Map<T1>("");
            group.Map<T2>("");
            group.Map<T3>("");
            return group;
        }

        public readonly RouteGroupBuilder With<T1,T2,T3,T4>()
            where T1 : IEndpoint, new()
            where T2 : IEndpoint, new()
            where T3 : IEndpoint, new()
            where T4 : IEndpoint, new()
        {
            var group = Builder.MapGroup(Pattern);
            group.Map<T1>("");
            group.Map<T2>("");
            group.Map<T3>("");
            group.Map<T4>("");
            return group;
        }

        public readonly RouteGroupBuilder With<T1,T2,T3,T4,T5>()
            where T1 : IEndpoint, new()
            where T2 : IEndpoint, new()
            where T3 : IEndpoint, new()
            where T4 : IEndpoint, new()
            where T5 : IEndpoint, new()
        {
            var group = Builder.MapGroup(Pattern);
            group.Map<T1>("");
            group.Map<T2>("");
            group.Map<T3>("");
            group.Map<T4>("");
            group.Map<T5>("");
            return group;
        }
    }

    public static RouteEndpointBinder Map(this IEndpointRouteBuilder routeBuilder, [StringSyntax("Route")]string pattern)
        => new(pattern, routeBuilder);

    public static RouteHandlerBuilder Map<T>(this IEndpointRouteBuilder routeBuilder, [StringSyntax("Route")]string pattern) where T : IEndpoint, new()
    {
        Type type = typeof(T);
        string name = type.Name;
        IEndpoint impl = new T();
        Delegate handler = impl.Handler;

        var handlerBuilder = impl switch
        {
            IGetEndpoint => routeBuilder.MapGet(pattern, handler),
            IPostEndpoint => routeBuilder.MapPost(pattern, handler),
            IPutEndpoint => routeBuilder.MapPut(pattern, handler),
            IPatchEndpoint => routeBuilder.MapPatch(pattern, handler),
            IDeleteEndpoint => routeBuilder.MapDelete(pattern, handler),
            _ => throw new InvalidOperationException("Must not be another case")
        };
        handlerBuilder.WithName(name);

        return handlerBuilder;
    }
}
