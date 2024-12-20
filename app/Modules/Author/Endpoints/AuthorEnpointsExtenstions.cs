using App.Data.Models;
using App.Endpoints.Models;
using App.Endpoints.Services;

namespace App.Endpoints;

public static class AuthorEnpointsExtenstions
{
    public static IEnumerable<GetAuthor> ToGetAuthors(this IEnumerable<Author> authors, EndpointContext context)
    {
        return authors.Select(x => x.ToGetAuthor(context));
    }

    public static GetAuthor ToGetAuthor(this Author author, EndpointContext context)
    {
        return author.ToGetAuthor(new
        {
            self = context.GetLink(AuthorEndpoints.GetAuthor, new { authorId = author.Id }),
            books = context.GetLink(AuthorEndpoints.GetAuthorBooks, new { authorId = author.Id }),
        });
    }
}
