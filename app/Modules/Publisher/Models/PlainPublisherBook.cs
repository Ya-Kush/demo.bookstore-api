using App.Data.Models;
using App.Endpoints.HypermediaPrimitives;
using App.Endpoints.Services;

namespace App.Endpoints.Models;

public readonly record struct PlainPublisherBook(Guid Id, string Title, string Edition, double Price) : IProps;

public static class PublisherBookConvertingExtensions
{
    public static PlainPublisherBook ToPublisherPlain(this Book book) => new (book.Id, book.Title, book.Edition, book.Price);
}

public static class PublisherBookHypermediaExtensions
{
    public static Act[] GetPublishersActs(this Book book, Publisher publisher, EndpointContext context)
    {
        var publisherId_bookId_Values = new { publisherId = publisher.Id, bookId = book.Id };
        Field[] removeBookFields = [];

        return [new(
            Name: "remove_book",
            Method: Act.Methods.DELETE,
            Href: context.GetLinkFor<DeletePublisherBook>(publisherId_bookId_Values),
            Fields: removeBookFields)];
    }
}
