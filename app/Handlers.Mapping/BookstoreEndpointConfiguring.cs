using App.Handlers.Authors;
using App.Handlers.Books;
using App.Handlers.Publishers;
using App.Handlers.Services;

namespace App.Handlers.Mapping;

public static class BookstoreConfiguring
{
    public static void AddBookstoreEndpointServices(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();
        services.AddScoped<EndpointContext>();
    }

    public static T MapBookstoreEndpoints<T>(this T router) where T : IEndpointRouteBuilder
    {
        const string bookId      = "{bookId:guid}";
        const string authorId    = "{authorId:guid}";
        const string publisherId = "{publisherId:guid}";

        var books = router.MapGroup("/books").WithTags("Book");
        var authors = router.MapGroup("/authors").WithTags("Author");
        var publishers = router.MapGroup("/publishers").WithTags("Publisher");

        books
            .Map("").WithMany()
                    .Add<GetBooks>(_ => _.AllowAnonymous())
                    .Last<PostBook>()
            .Map($"/{bookId}").WithMany()
                    .Add<GetBook>(_ => _.AllowAnonymous())
                    .Add<PatchBook>()
                    .Last<DeleteBook>()
            .Map($"/{bookId}/authors").WithMany()
                    .Add<GetBookAuthors>(_ => _.AllowAnonymous())
                    .Last<PostBookAuthor>()
            .Map($"/{bookId}/author/{authorId}").WithCompleted<DeleteBookAuthor>();

        authors
            .Map("").WithCompleted<GetAuthors>(_ => _.AllowAnonymous())
            .Map("").WithCompleted<PostAuthor>()
            .Map($"/{authorId}").WithCompleted<GetAuthor>(_ => _.AllowAnonymous())
            .Map($"/{authorId}").WithCompleted<PatchAuthor>()
            .Map($"/{authorId}").WithCompleted<DeleteAuthor>()
            .Map($"/{authorId}/books").WithCompleted<GetAuthorBooks>(_ => _.AllowAnonymous())
            .Map($"/{authorId}/books").WithCompleted<PostAuthorBook>()
            .Map($"/{authorId}/books/{bookId}").With<DeleteAuthorBook>();

        publishers.Map("").With<GetPublishers>().AllowAnonymous();
        publishers.Map("").With<PostPublisher>();
        publishers.Map($"/{publisherId}").With<GetPublisher>().AllowAnonymous();
        publishers.Map($"/{publisherId}").With<PatchPublisher>();
        publishers.Map($"/{publisherId}").With<DeletePublisher>();
        publishers.Map($"/{publisherId}/books").With<GetPublisherBooks>().AllowAnonymous();
        publishers.Map($"/{publisherId}/books").With<PostPublisherBook>();
        publishers.Map($"/{publisherId}/books/{bookId}").With<DeletePublisherBook>();

        return router;
    }
}