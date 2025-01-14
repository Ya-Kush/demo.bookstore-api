using App.Data.Models;
using App.Endpoints.HypermediaPrimitives;
using App.Endpoints.Services;

namespace App.Endpoints.Models;

public readonly record struct PlainAuthorBook(Guid Id, string Title, string Edition, Guid? PublisherId, double Price) : IProps;

public static class AuthorBookConvertingExtensions
{
    public static PlainAuthorBook ToAuthorPlain(this Book book) => new(book.Id, book.Title, book.Edition, book.Publisher?.Id, book.Price);
}

public static class AuhtorBookHypermediaExtensions
{
    public static Act[] GetAuthorActs(this Book book, Author author, EndpointContext context)
    {
        return [new(
            Name: "remove_book",
            Method: Act.Methods.DELETE,
            Href: context.GetLinkFor<DeleteAuthorBook>(new { bookId = book.Id, authorId = author.Id }),
            Fields: [])];
    }
}