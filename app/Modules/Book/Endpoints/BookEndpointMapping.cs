namespace App.Endpoints;

public static class BookEndpointMapping
{
    public static RouteGroupBuilder MapBooks(this IEndpointRouteBuilder routeBuilder)
    {
        var group = routeBuilder.MapGroup("/books").WithTags("Book");

        var bookId = "{bookId:guid}";
        var authorId = "{authorId:guid}";

        group.Map("").With<GetBooks,PostBook>();
        group.Map($"/{bookId}").With<GetBook,PatchBook,DeleteBook>();
        group.Map($"/{bookId}/authors").With<GetBookAuthors,PostBookAuthor>();
        group.Map($"/{bookId}/author/{authorId}").With<DeleteBookAuthor>();

        return group;
    }
}
