namespace App.DomainModels;

public sealed class Author : IBookstoreModel
{
    public Guid Id { get; }
    public string FirstName { get; set; }
    public string MiddleName { get; set; }
    public string LastName { get; set; }

    readonly List<Book> _books = [];

    public IReadOnlyCollection<Book> Books
    {
        get => _books.AsReadOnly();
        init { _books = [..value]; }
    }

    private Author(Guid id, string firstName, string middleName, string lastName)
        => (Id, FirstName, MiddleName, LastName) = (id, firstName, middleName, lastName);

    public static Author New(string firstName, string middleName, string lastName, List<Book> books)
        => new(Guid.NewGuid(), firstName, middleName, lastName) { Books = books };

    public void AddBooks(params Book[] books)
    {
        foreach (var b in books)
        {
            if (_books.Any(x => x.Id == b.Id) is false) _books.Add(b);
            if (b.Authors.Any(x => x.Id == Id) is false) b.AddAuthors(this);
        }
    }
}
