using App.Data.Models;
using App.Handlers.HypermediaPrimitives;
using App.Handlers.Services;

namespace App.Handlers.Publishers.Models;

public readonly record struct PlainPublisher(Guid Id, string Name) : IProps;

public static class PublisherConvertingExtensions
{
    public static PlainPublisher ToPlain(this Publisher pub) => new(pub.Id, pub.Name);
}

public static class PublisherHypermediaExtensions
{
    public static Link[] GetLinks(this Publisher publisher, EndpointContext context)
    {
        var publisherId_Values = new { publisherId = publisher.Id };
        return [
            new(Rel: "self",
                Href: context.GetLink(nameof(GetPublisher), publisherId_Values)),

            new(Rel: "books",
                Href: context.GetLink(nameof(GetPublisherBooks), publisherId_Values))
        ];
    }

    public static Act[] GetActs(this Publisher publisher, EndpointContext context)
    {
        var publisherId_Values = new { publisherId = publisher.Id };
        Field[] changeFields = [new("name", "string")];
        Field[] deleteFields = [];

        return [
            new(Name: "change",
                Method: Act.Methods.PATCH,
                Href: context.GetLinkFor<PatchPublisher>(publisherId_Values),
                Fields: changeFields),

            new(Name: "delete",
                Method: Act.Methods.DELETE,
                Href: context.GetLinkFor<DeletePublisher>(publisherId_Values),
                Fields: deleteFields),
        ];
    }
}
