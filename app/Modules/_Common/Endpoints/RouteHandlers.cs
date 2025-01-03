using System.Diagnostics.CodeAnalysis;

namespace App.Endpoints;

public readonly record struct RouteHandlers([StringSyntax("Route")] string Pattern, params Delegate[] Handlers)
{
    public readonly ICollection<RouteHandlers> Sub { get; } = [];
    public RouteHandlers AddSub(params RouteHandlers[] routes)
    {
        foreach (var r in routes) Sub.Add(new(Pattern + r.Pattern, Handlers));
        return this;
    }
}
