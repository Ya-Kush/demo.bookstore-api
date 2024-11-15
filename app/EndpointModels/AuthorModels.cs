using App.DomainModels;

namespace App.EndpointModels;

public readonly record struct GetAuthor(Guid Id, string FirstName, string MiddleName, string LastName, object? Links) : IGetEndpointModel;
public readonly record struct PostAuthor(string FirstName, string MiddleName, string LastName) : IPostEndpointModel;
public readonly record struct PutAuthor(string FirstName, string MiddleName, string LastName) : IPutEndpointModel;
public readonly record struct PatchAuthor(string? FirstName, string? MiddleName, string? LastName) : IPatchEndpointModel;

public static class AuthorConvertorExtensions
{
    public static GetAuthor ToGetAuthor(this Author author)
        => new(author.Id, author.FirstName, author.MiddleName, author.LastName, null);

    public static GetAuthor ToGetAuthor(this Author author, object links)
        => new(author.Id, author.FirstName, author.MiddleName, author.LastName, links);

    public static GetAuthor ToGetAuthor(this Author author, Func<Author, object> linkGenerator)
        => new(author.Id, author.FirstName, author.MiddleName, author.LastName, linkGenerator(author));

    public static Author ToAuthor(this PostAuthor postAuthor)
        => Author.New(postAuthor.FirstName, postAuthor.MiddleName, postAuthor.LastName, []);

    public static Author ToAuthor(this PutAuthor postAuthor)
        => Author.New(postAuthor.FirstName, postAuthor.MiddleName, postAuthor.LastName, []);

    public static Author Swap(this Author author, PutAuthor putAuthor)
    {
        author.FirstName = putAuthor.FirstName;
        author.MiddleName = putAuthor.MiddleName;
        author.LastName = putAuthor.LastName;

        return author;
    }

    public static Author Update(this Author author, PatchAuthor patchAuthor)
    {
        if (patchAuthor.FirstName is not null) author.FirstName = patchAuthor.FirstName;
        if (patchAuthor.MiddleName is not null) author.MiddleName = patchAuthor.MiddleName;
        if (patchAuthor.LastName is not null) author.LastName = patchAuthor.LastName;

        return author;
    }

    public static PutAuthor ToPutAuthor(this PostAuthor postAuthor)
        => new(postAuthor.FirstName, postAuthor.MiddleName, postAuthor.LastName);

    public static PostAuthor ToPostAuthor(this PutAuthor putAuthor)
        => new(putAuthor.FirstName, putAuthor.MiddleName, putAuthor.LastName);
}
