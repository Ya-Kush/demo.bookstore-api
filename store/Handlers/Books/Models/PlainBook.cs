using Store.Data.Models;
using Store.Handlers.HypermediaPrimitives;
using Store.Handlers.Services;

namespace Store.Handlers.Books.Models;

public readonly record struct PlainBook(Guid Id, string Title, string Edition, Guid? PublisherId, double Price) : IProps;

public static class BookConvertingExtensions
{
    public static PlainBook ToPlain(this Book book) => new(book.Id, book.Title, book.Edition, book.Publisher?.Id, book.Price);
}

public static class BookHypermediaExtesions
{
    public static Link[] GetLinks(this Book book, EndpointContext context)
    {
        var bookId_Values = new { bookId = book.Id };
        return [
            new(Rel: "self",
                Href: context.GetLinkFor<GetBook>(bookId_Values)),

            new(Rel: "authors",
                Href: context.GetLinkFor<GetBookAuthors>(bookId_Values))
        ];
    }

    public static Act[] GetActs(this Book book, EndpointContext context)
    {
        var bookId_Values = new { bookId = book.Id };
        Field[] changeFields = [
            new("title", "string?"),
            new("edition", "string?"),
            new("price", "double?"),
            new("publisherId", "guid?")
        ];
        Field[] deleteFields = [];

        return [
            new(Name: "change",
                Method: Act.Methods.PATCH,
                Href: context.GetLinkFor<PatchBook>(bookId_Values),
                Fields: changeFields),

            new(Name: "delete",
                Method: Act.Methods.DELETE,
                Href: context.GetLinkFor<DeleteBook>(bookId_Values),
                Fields: deleteFields),
        ];
    }
}
