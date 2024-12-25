using App.Data.Models;

namespace App.Endpoints.Models;

public readonly record struct SimplePublisherResponse(Guid Id, string Name, IEnumerable<Link> _links);

public static class PublisherConvertorExtensions
{
    public static SimplePublisherResponse ToSimplePublisherResponse(this Publisher pub, IEnumerable<Link> links) => new(pub.Id, pub.Name, links);
    public static SimplePublisherResponse ToSimplePublisherResponse(this Publisher pub, params Link[] links) => new(pub.Id, pub.Name, links);
}
