using App.DomainModels;

namespace App.EndpointModels;

public record struct GetAuthor(Guid Id, string FirstName, string MiddleName, string LastName, object? Links) : IGetEndpointModel;

public static class AuthorConvertorExtensions
{
    public static GetAuthor ToGetAuthor(this Author author, object? links = null)
    {
        return new(author.Id, author.FirstName, author.MiddleName, author.LastName, links);
    }
}