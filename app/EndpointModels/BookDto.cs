using App.DomainModels;

namespace App.EndpointModels;

public record struct ResponseBook(Guid Guid, string Title, string Edition, double Price);
public record struct ReceivedNewBook(string Title, string Edition, double Price);
public record struct ModifiedBook(string? Title, string? Edition, double? Price);

public static class BookModelConvertorExtensions
{
    public static ResponseBook ToResponse(this Book book) =>
        new(book.Guid, book.Title, book.Edition, book.Price);

    public static Book ToBook(this ReceivedNewBook received) => 
        Book.New(received.Title, received.Edition, [], "unknow", "unreleased", received.Price);

    public static Book Modify(this Book book, ModifiedBook modified)
    {
        if (modified.Title is not null) book.Title = modified.Title;
        if (modified.Edition is not null) book.Edition = modified.Edition;
        if (modified.Price is not null) book.Price = modified.Price.Value;

        return book;
    }
}