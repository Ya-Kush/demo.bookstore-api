using App.Data.Models;

namespace App.Endpoints.Models;

public readonly record struct GetBook(Guid Id, string Title, string Edition, SimplePublisherResponse? Publisher, double Price, IEnumerable<Link> _links);
public readonly record struct PostBook(string Title, string Edition, double Price);
public readonly record struct PutBook(string Title, string Edition, double Price);
public readonly record struct PatchBook(string? Title, string? Edition, double? Price);

public static class BookModelConvertorExtensions
{
    public static GetBook ToGetBook(this Book book, params Link[] links)
        => new(book.Id, book.Title, book.Edition, book.Publisher?.ToSimplePublisherResponse(), book.Price, links);

    public static GetBook ToGetBook(this Book book, Func<Book, IEnumerable<Link>> linkGenerator)
        => new(book.Id, book.Title, book.Edition, book.Publisher?.ToSimplePublisherResponse(), book.Price, linkGenerator(book));

    public static Book ToBook(this PostBook postBook)
        => Book.New(postBook.Title, postBook.Edition, postBook.Price);

    public static Book ToBook(this PutBook putBook)
        => Book.New(putBook.Title, putBook.Edition, putBook.Price);

    public static Book Swap(this Book book, PutBook putBook)
    {
        book.Title = putBook.Title;
        book.Edition = putBook.Edition;
        book.Price = putBook.Price;

        return book;
    }

    public static Book Update(this Book book, PatchBook patchBook)
    {
        if (patchBook.Title is not null) book.Title = patchBook.Title;
        if (patchBook.Edition is not null) book.Edition = patchBook.Edition;
        if (patchBook.Price is not null) book.Price = patchBook.Price.Value;

        return book;
    }

    public static PutBook ToPutBook(this PostBook postBook)
        => new(postBook.Title, postBook.Edition, postBook.Price);

    public static PostBook ToPostBook(this PutBook putBook)
        => new(putBook.Title, putBook.Edition, putBook.Price);
}
