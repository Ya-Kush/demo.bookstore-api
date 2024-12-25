using App.Data.Models;
using App.Endpoints.Models;
using App.Endpoints.Services;

namespace App.Endpoints;

public static class Ex
{
    public static IEnumerable<SimplePublisherResponse> ToSimplePublisherResponses(this IEnumerable<Publisher> authors, EndpointContext context)
    {
        return authors.Select(x => x.ToSimplePublisherResponse(context));
    }

    public static SimplePublisherResponse ToSimplePublisherResponse(this Publisher publisher, EndpointContext context)
    {
        return publisher.ToSimplePublisherResponse(
            new(Rel: "self",
                Href: context.GetLink(nameof(GetPublisher), new { publisherId = publisher.Id })),
            new(Rel: "books",
                Href: context.GetLink(nameof(GetPublisherBooks), new { publisherId = publisher.Id }))
        );
    }
}