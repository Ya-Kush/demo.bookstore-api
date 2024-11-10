namespace App.DomainModels;

public sealed class Book : IBookstoreModel
{
    public Guid Id { get; }
    public string Title { get; set; }
    public string Edition { get; set; }
    public string Publisher { get; set; }
    public string Released { get; set; }
    public double Price { get; set; }

    readonly List<Author> _authors = [];

    public IReadOnlyCollection<Author> Authors
    {
        get => _authors.AsReadOnly();
        init { _authors = [..value]; }
    }

    private Book(Guid id, string title, string edition, string publisher, string released, double price)
        => (Id, Title, Edition, Publisher, Released, Price) = (id, title, edition, publisher, released, price);

    public static Book New(string title, string edition, List<Author> authors, string publisher, string released, double price)
        => new(Guid.NewGuid(), title, edition, publisher, released, price) { Authors = authors };

    public void AddAuthors(params Author[] authors)
    {
        foreach (var a in authors)
        {
            if (_authors.Any(x => x.Id == a.Id) is false) _authors.Add(a);
            if (a.Books.Any(x => x.Id == Id) is false) a.AddBooks(this);
        }
    }
}

