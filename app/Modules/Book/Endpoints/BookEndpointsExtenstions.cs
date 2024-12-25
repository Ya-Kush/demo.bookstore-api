using App.Data.Models;
using App.Endpoints.Models;
using App.Endpoints.Services;

namespace App.Endpoints;

public static class BookEndpointsExtenstions
{
    public static IEnumerable<GetBook> ToGetBooks(this IEnumerable<Book> books, EndpointContext context)
    {
        return books.Select(x => x.ToGetBook(context));
    }

    public static GetBook ToGetBook(this Book book, EndpointContext context)
    {
        return book.ToGetBook(
            new(Rel: "self",
                Href: context.GetLink(BookEndpoints.GetBook, new { bookId = book.Id })),
            new(Rel: "authors",
                Href: context.GetLink(BookEndpoints.GetBookAuthors, new { bookId = book.Id }))
        );
    }
}
