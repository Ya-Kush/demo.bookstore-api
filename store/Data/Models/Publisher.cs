using Store.Common;
using Store.Common.Extensions;

namespace Store.Data.Models;

public sealed class Publisher : IBookstoreModel
{
    public Guid Id { get; }

    public string Name { get; set; }

    readonly List<Book> _books = [];
    public IEnumerable<Book> Books => _books;

    internal Publisher(Guid id, string name)
        => (Id, Name) = (id, name);

    public static Res<Publisher> New(string name)
        => name.IsNullOrWhiteSpace()
        ? new Error("Wrong name format")
        : new Publisher(Guid.NewGuid(), name);

    public Publisher WithBooks(IEnumerable<Book> books)
    {
        AddBooks(books);
        return this;
    }

    public bool HasBook(Book book) => _books.Any(b => b.Id == book.Id);
    public bool HasBook(Guid id) => _books.Any(b => b.Id == id);
    public bool HasEachOther(Book book)
    {
        var (hb, hp) = (HasBook(book), book.Publisher?.Id == this.Id);
        if (hb ^ hp) throw new InvalidOperationException(message: $"desync data in {this} and {book} objects");
        return hb && hp;
    }


    public void AddBooks(IEnumerable<Book> books)
    {
        foreach (var b in books) AddBook(b);
    }
    public void AddBook(Book book)
    {
        if (_books.Any(b => b.Id == book.Id) is false)
            _books.Add(book);
        book.Publisher = this;
    }

    public void RemoveBooks(IEnumerable<Book> books)
    {
        foreach (var b in books) RemoveBook(b);
    }
    public void RemoveBook(Book book)
    {
        if (_books.Any(x => x.Id == book.Id))
            _books.Remove(book);
        book.UnsetPublisher();
    }
}