namespace App.DomainModels;

public sealed class Author : IBookstoreModel
{
    public Guid Id { get; }
    public string FirstName { get; set; }
    public string MiddleName { get; set; }
    public string LastName { get; set; }

    readonly List<Book> _books = [];

    public IEnumerable<Book> Books
    {
        get => _books.AsReadOnly();
        init { _books = [..value]; }
    }

    private Author(Guid id, string firstName, string middleName, string lastName)
        => (Id, FirstName, MiddleName, LastName) = (id, firstName, middleName, lastName);

    public static Author New(string firstName, string middleName, string lastName, List<Book> books)
        => new(Guid.NewGuid(), firstName, middleName, lastName) { Books = books };

    public static Author NewWithId(Guid id, string firstName, string middleName, string lastName, List<Book> books)
        => new(id, firstName, middleName, lastName) { Books = books };


    public void AddBook(Book book)
    {
        if (_books.Any(x => x.Id == book.Id)) return;

        _books.Add(book);
        book.AddAuthor(this);
    }
    public void AddBooks(IEnumerable<Book> books)
    {
        foreach (var b in books) AddBook(b);
    }

    public void RemoveBook(Book book)
    {
        if (_books.All(x => x.Id != book.Id)) return;

        _books.Remove(book);
        book.RemoveAuthor(this);
    }
    public void RemoveBooks(IEnumerable<Book> books)
    {
        foreach (var b in books) RemoveBook(b);
    }
}
