namespace App.Data.Models;

public sealed class Publisher : IBookstoreModel
{
    public Guid Id { get; }

    string _name;
    public string Name
    {
        get => _name;
        set
        {
            if (Id == Guid.Empty) throw new InvalidOperationException("Attempt to change property of a readonly object");
            _name = value;
        }
    }

    readonly List<Book> _books = [];
    public IEnumerable<Book> Books => _books;

    public readonly static Publisher Default = new(Guid.Empty, "Unknown");

    internal Publisher(Guid id, string name)
        => (Id, _name) = (id, name);

    public static Publisher New(string name)
        => new(Guid.NewGuid(), name);

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
        if (_books.Any(b => b.Id == book.Id)) return;

        _books.Add(book);
        book.Publisher = this;
    }

    public void RemoveBooks(IEnumerable<Book> books)
    {
        foreach (var b in books) RemoveBook(b);
    }
    public void RemoveBook(Book book)
    {
        if (_books.All(x => x.Id != book.Id)) return;

        _books.Remove(book);
        book.UnsetPublisher();
    }
}
