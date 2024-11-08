using App.DomainModels;

namespace App.EndpointModels;

public record struct ResponseAuthor(Guid Guid, string FirstName, string MiddleName, string LastName);

public static class AuthorConvertorExtensions
{
    public static ResponseAuthor ToResponse(this Author author)
    {
        return new(author.Guid, author.FirstName, author.MiddleName, author.LastName);
    }
}