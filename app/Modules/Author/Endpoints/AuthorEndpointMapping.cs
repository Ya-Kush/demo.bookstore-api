namespace App.Endpoints;

public static class AuthorEndpointMapping
{
    public static RouteGroupBuilder MapAuthors(this IEndpointRouteBuilder routeBuilder)
    {
        var group = routeBuilder.MapGroup("/authors").WithTags("Author");

        var authorId = "{authorId:guid}";
        var bookId = "{bookId:guid}";

        group.Map("").With<GetAuthors, PostAuthor>();
        group.Map($"/{authorId}").With<GetAuthor, PatchAuthor, DeleteAuthor>();
        group.Map($"/{authorId}/books").With<GetAuthorBooks, PostAuthorBook>();
        group.Map($"/{authorId}/books/{bookId}").With<DeleteAuthorBook>();

        return group;
    }
}