namespace App.Endpoints;

public static class PublisherEndpointsMapping
{
    public static RouteGroupBuilder MapPublishers(this IEndpointRouteBuilder routeBuilder)
    {
        var group = routeBuilder.MapGroup("/publishers").WithTags("Publisher");

        group.MapGet("", GetPublishers.Handle).WithName(nameof(GetPublisher));
        group.MapGet("/{publisherId:guid}", GetPublisher.Handle).WithName(nameof(GetPublishers));
        group.MapGet("/{publisherId:guid}/books", GetPublisherBooks.Handle).WithName(nameof(GetPublisherBooks));

        return group;
    }
}
