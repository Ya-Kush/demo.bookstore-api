using App.Data.Models;

namespace App.Endpoints.Models;

public readonly record struct FlatPublisher(Guid Id, string Name);

public static class PublisherConvertorExtensions
{
    public static FlatPublisher ToFlat(this Publisher pub) => new(pub.Id, pub.Name);
}
