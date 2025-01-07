using App.Data.Models;
using App.Endpoints.Services;

namespace App.Endpoints.Models;

public static class PublisherEndpointModelExtensions
{
    public static Link[] GetLinks(this Publisher publisher, EndpointContext context)
    {
        return
        [
            new(Rel: "self",
                Href: context.GetLink(nameof(GetPublisher), new { publisherId = publisher.Id })),
            new(Rel: "books",
                Href: context.GetLink(nameof(GetPublisherBooks), new { publisherId = publisher.Id }))
        ];
    }

    public static Act[] GetActs(this Publisher publisher, EndpointContext context)
    {
        var publisherId_Values = new { publisherId = publisher.Id };

        return
        [
            new(Rel: "change",
                Method: Act.Methods.PATCH,
                Href: context.GetLinkBy<PatchPublisher>(publisherId_Values)),
            new(Rel: "delete",
                Method: Act.Methods.DELETE,
                Href: context.GetLinkBy<DeletePublisher>(publisherId_Values)),
        ];
    }

    public static Link[] GetLinks(this Book book, EndpointContext context)
    {
        return
        [
            new(
                Rel: "self",
                Href: context.GetLink(BookEndpoints.GetBook, new { bookId = book.Id }))
        ];
    }

    public static Act[] GetPublishersActs(this Book book, Publisher publisher, EndpointContext context)
    {
        var publisherId_bookId_Values = new { publisherId = publisher.Id, bookId = book.Id };
        return
        [
            new(Rel: "remove_book",
            Method: Act.Methods.DELETE,
            Href: context.GetLinkBy<DeletePublisherBook>(publisherId_bookId_Values)),

            new(Rel: "add_book",
            Method: Act.Methods.PUT,
            Href: context.GetLinkBy<PutPublisherBook>(publisherId_bookId_Values))
        ];
    }
}