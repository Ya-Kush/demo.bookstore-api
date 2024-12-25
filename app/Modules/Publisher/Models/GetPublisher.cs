using App.Data.Models;

namespace App.Endpoints.Models;

public readonly record struct GetPublisher(Guid Id, string Name);

public static class PublisherConvertorExtensions
{
    public static GetPublisher ToGetPublisher(this Publisher pub) => new(pub.Id, pub.Name);
}
