using App.DomainModels;

namespace App.EndpointModels;

public record struct GetAuthor(Guid Id, string FirstName, string MiddleName, string LastName, object? Links) : IGetEndpointModel;
public readonly record struct PostAuthor(string FirstName, string MiddleName, string LastName) : IPostEndpointModel;
public readonly record struct PutAuthor(string FirstName, string MiddleName, string LastName) : IPutEndpointModel;
public readonly record struct PatchAuthor(string? FirstName, string? MiddleName, string? LastName) : IPatchEndpointModel;

public static class AuthorConvertorExtensions
{
    public static GetAuthor ToGetAuthor(this Author author)
        => new(author.Id, author.FirstName, author.MiddleName, author.LastName, null);

    public static Author ToAuthor(this PostAuthor postAuthor)
        => Author.New(postAuthor.FirstName, postAuthor.MiddleName, postAuthor.LastName, []);

    public static Author ToAuthor(this PutAuthor postAuthor)
        => Author.New(postAuthor.FirstName, postAuthor.MiddleName, postAuthor.LastName, []);

    public static Author Update(this Author author, PatchAuthor patchAuthor)
    {
        if (patchAuthor.FirstName is not null) author.FirstName = patchAuthor.FirstName;
        if (patchAuthor.MiddleName is not null) author.MiddleName = patchAuthor.MiddleName;
        if (patchAuthor.LastName is not null) author.LastName = patchAuthor.LastName;

        return author;
    }

    public static Author Swap(this Author author, PutAuthor postAuthor)
    {
        author.FirstName = postAuthor.FirstName;
        author.MiddleName = postAuthor.MiddleName;
        author.LastName = postAuthor.LastName;

        return author;
    }
}