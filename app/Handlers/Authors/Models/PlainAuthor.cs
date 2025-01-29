using App.Data.Models;
using App.Handlers.HypermediaPrimitives;
using App.Handlers.Services;

namespace App.Handlers.Authors.Models;

public readonly record struct PlainAuthor(Guid Id, string FirstName, string MiddleName, string LastName) : IProps;

public static class AuthorConvertingExtensions
{
    public static PlainAuthor ToPlain(this Author author) => new(author.Id, author.FirstName, author.MiddleName, author.LastName);
}

public static class AuthorHypermediaExtensions
{
    public static Link[] GetLinks(this Author author, EndpointContext context)
    {
        object authorIdValues = new { authorId = author.Id };

        return [
            new(Rel: "self",
                Href: context.GetLinkFor<GetAuthor>(authorIdValues)),
            new(Rel: "books",
                Href: context.GetLinkFor<GetAuthorBooks>(authorIdValues))
        ];
    }

    public static Act[] GetActs(this Author author, EndpointContext context)
    {
        object authorIdValues = new { authorId = author.Id };
        Field[] changeFields = [new("firstName", "string"), new("middleName", "string"), new("lastName", "string")];
        Field[] deleteFields = [];

        return[
            new(Name: "change",
                Method: Act.Methods.PATCH,
                Href: context.GetLinkFor<PatchAuthor>(authorIdValues),
                Fields: changeFields),
            new(Name: "delete",
                Method: Act.Methods.DELETE,
                Href: context.GetLinkFor<DeleteAuthor>(authorIdValues),
                Fields: deleteFields)
        ];
    }
}