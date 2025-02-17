using Store.Data.Models;
using Store.Handlers.HypermediaPrimitives;
using Store.Handlers.Services;

namespace Store.Handlers.Books.Models;

public readonly record struct PlainBookAuthor(Guid Id, string FirstName, string MiddleName, string LastName) : IProps;

public static class BookAuthorConvertingExtensions
{
    public static PlainBookAuthor ToBookPlain(this Author author) => new(author.Id, author.FirstName, author.MiddleName, author.LastName);
}

public static class BookAuthorHypermediaExtensions
{
    public static Act[] GetBookActs(this Author author, Book book, EndpointContext context)
    {
        return [new(
            Name: "remove_author",
            Method: Act.Methods.DELETE,
            Href: context.GetLinkFor<DeleteBookAuthor>(new { bookId = book.Id, authorId = author.Id }),
            Fields: [])];
    }
}