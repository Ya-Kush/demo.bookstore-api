using App.DomainModels;

namespace App.EndpointModels;

public record struct GetBook(Guid Id, string Title, string Edition, double Price, object? Links) : IGetEndpointModel;
public readonly record struct PostBook(string Title, string Edition, double Price) : IPostEndpointModel;
public readonly record struct PutBook(string Title, string Edition, double Price) : IPutEndpointModel;
public readonly record struct PatchBook(string? Title, string? Edition, double? Price) : IPatchEndpointModel;

public static class BookModelConvertorExtensions
{
    public static GetBook ToGetBook(this Book book)
        => new(book.Id, book.Title, book.Edition, book.Price, null);

    public static Book ToBook(this PostBook received)
        => Book.New(received.Title, received.Edition, [], "unknow", "unreleased", received.Price);

    public static Book ToBook(this PutBook received)
        => Book.New(received.Title, received.Edition, [], "unknow", "unreleased", received.Price);

    public static Book Update(this Book book, PatchBook modified)
    {
        if (modified.Title is not null) book.Title = modified.Title;
        if (modified.Edition is not null) book.Edition = modified.Edition;
        if (modified.Price is not null) book.Price = modified.Price.Value;

        return book;
    }

    public static Book Swap(this Book book, PutBook postBook)
    {
        book.Title = postBook.Title;
        book.Edition = postBook.Edition;
        book.Price = postBook.Price;

        return book;
    }
}
