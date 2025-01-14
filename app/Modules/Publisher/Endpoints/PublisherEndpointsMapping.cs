namespace App.Endpoints;

public static class PublisherEndpointsMapping
{
    public static RouteGroupBuilder MapPublishers(this IEndpointRouteBuilder routeBuilder)
    {
        var group = routeBuilder.MapGroup("/publishers").WithTags("Publisher");

        var publisherId = "{publisherId:guid}";
        var bookId = "{bookId:guid}";

        group.Map("").With<GetPublishers,PostPublisher>();
        group.Map($"/{publisherId}").With<GetPublisher,PatchPublisher,DeletePublisher>();
        group.Map($"/{publisherId}/books").With<GetPublisherBooks,PostPublisherBook>();
        group.Map($"/{publisherId}/books/{bookId}").With<DeletePublisherBook>();

        return group;
    }
}
