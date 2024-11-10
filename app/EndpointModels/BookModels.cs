using App.DomainModels;

namespace App.EndpointModels;

public record struct GetBook(Guid Id, string Title, string Edition, double Price, object? Links) : IGetEndpointModel;
public record struct PostBook(string Title, string Edition, double Price);
public record struct PatchBook(string? Title, string? Edition, double? Price);

public static class BookModelConvertorExtensions
{
    public static GetBook ToGetBook(this Book book) =>
        new(book.Id, book.Title, book.Edition, book.Price, null);

    public static Book ToBook(this PostBook received) => 
        Book.New(received.Title, received.Edition, [], "unknow", "unreleased", received.Price);

    public static Book Update(this Book book, PatchBook modified)
    {
        if (modified.Title is not null) book.Title = modified.Title;
        if (modified.Edition is not null) book.Edition = modified.Edition;
        if (modified.Price is not null) book.Price = modified.Price.Value;

        return book;
    }

    public static Book Update(this Book book, PostBook newBook)
    {
        book.Title = newBook.Title;
        book.Edition = newBook.Edition;
        book.Price = newBook.Price;

        return book;
    }
}