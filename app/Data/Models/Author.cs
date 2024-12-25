namespace App.Data.Models;

public sealed class Author : IBookstoreModel
{
    public Guid Id { get; }
    public string FirstName { get; set; }
    public string MiddleName { get; set; }
    public string LastName { get; set; }

    readonly List<Book> _books = [];
    public IEnumerable<Book> Books
    {
        get => _books;
        init => _books = [..value];
    }

    internal Author(Guid id, string firstName, string middleName, string lastName)
        => (Id, FirstName, MiddleName, LastName) = (id, firstName, middleName, lastName);

    public static Author New(string firstName, string middleName, string lastName)
        => new(Guid.NewGuid(), firstName, middleName, lastName);

    public Author WithBooks(IEnumerable<Book> books)
    {
        AddBooks(books);
        return this;
    }

    public bool HasBook(Book book) => _books.Any(b => b.Id == book.Id);
    public bool HasBook(Guid id) => _books.Any(b => b.Id == id);
    public bool HasEachOther(Book book)
    {
        var (ha, hb) = (HasBook(book), book.HasAuthor(this));
        if (ha ^ hb) throw new InvalidOperationException(message: $"desync data in {this} and {book} objects");
        return ha && hb;
    }


    public void AddBooks(IEnumerable<Book> books)
    {
        foreach (var b in books) AddBook(b);
    }
    public void AddBook(Book book)
    {
        if (_books.Any(x => x.Id == book.Id)) return;

        _books.Add(book);
        book.AddAuthor(this);
    }

    public void RemoveBooks(IEnumerable<Book> books)
    {
        foreach (var b in books) RemoveBook(b);
    }
    public void RemoveBook(Book book)
    {
        if (_books.All(x => x.Id != book.Id)) return;

        _books.Remove(book);
        book.RemoveAuthor(this);
    }
}
