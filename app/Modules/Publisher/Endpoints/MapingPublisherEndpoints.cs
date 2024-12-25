namespace App.Endpoints;

public static class MapingPublisherEndpoints
{
    public static RouteGroupBuilder MapPublishers(this IEndpointRouteBuilder routeBuilder)
    {
        var group = routeBuilder.MapGroup("/publishers").WithTags("Publisher");

        group.MapGet("", GetPublishers.Handler).WithName(nameof(GetPublisher));
        group.MapGet("/{publisherId:guid}"!, GetPublisher.Handler).WithName(nameof(GetPublishers));
        group.MapGet("/{publisherId:guid}/books"!, GetPublisherBooks.Handler).WithName(nameof(GetPublisherBooks));

        return group;
    }
}
