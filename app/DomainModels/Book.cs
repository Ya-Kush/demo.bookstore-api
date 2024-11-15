namespace App.DomainModels;

public sealed class Book : IBookstoreModel
{
    public Guid Id { get; }
    public string Title { get; set; }
    public string Edition { get; set; }
    // public string Publisher { get; set; }
    // public string Released { get; set; }
    public double Price { get; set; }

    readonly List<Author> _authors = [];

    public IEnumerable<Author> Authors
    {
        get => _authors.AsReadOnly();
        init { _authors = [..value]; }
    }

    private Book(Guid id, string title, string edition, double price)
        => (Id, Title, Edition, Price) = (id, title, edition, price);

    public static Book New(string title, string edition, double price, List<Author> authors)
        => new(Guid.NewGuid(), title, edition, price) { Authors = authors };

    public static Book NewWithoutAuthors(string title, string edition, double price)
        => New(title, edition, price, []);

    internal static Book NewWithId(Guid id, string title, string edition, double price, List<Author> authors)
        => new(id, title, edition, price) { Authors = authors };

    internal static Book NewWithIdWithoutAuthors(Guid id, string title, string edition, double price)
        => NewWithId(id, title, edition, price, []);


    public void AddAuthor(Author author)
    {
        if (_authors.Any(x => x.Id == author.Id))

        _authors.Add(author);
        author.AddBook(this);
    }
    public void AddAuthors(IEnumerable<Author> authors)
    {
        foreach (var a in authors) AddAuthor(a);
    }

    public void RemoveAuthor(Author author)
    {
        if (_authors.All(x => x.Id != author.Id)) return;

        _authors.Remove(author);
        author.RemoveBook(this);
    }
    public void RemoveAuthors(IEnumerable<Author> authors)
    {
        foreach (var a in authors) RemoveAuthor(a);
    }
}
