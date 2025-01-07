namespace App.Endpoints;

public static class PublisherEndpointsMapping
{
    public static RouteGroupBuilder MapPublishers(this IEndpointRouteBuilder routeBuilder)
    {
        var group = routeBuilder.MapGroup("/publishers").WithTags("Publisher");

        var publisherId = "{publisherId:guid}";
        var bookId = "{bookId}";

        group.Map("").With<GetPublishers,PostPublisher>();
        group.Map($"/{publisherId}").With<GetPublisher,PatchPublisher,DeletePublisher>();
        group.Map($"/{publisherId}/books").With<GetPublisherBooks>();
        group.Map($"/{publisherId}/books/{bookId}").With<PutPublisherBook,DeletePublisherBook>();

        return group;
    }
}
