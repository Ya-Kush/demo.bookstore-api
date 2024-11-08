namespace App.DomainModels;

public sealed class Book
{
    public Guid Guid { get; }
    public string Title { get; set; }
    public string Edition { get; set; }
    public string Publisher { get; set; }
    public string Released { get; set; }
    public double Price { get; set; }

    public List<Author> Authors { get; init; } = [];

    private Book(Guid guid, string title, string edition, string publisher, string released, double price)
        => (Guid, Title, Edition, Publisher, Released, Price) = (guid, title, edition, publisher, released, price);

    public static Book New(string title, string edition, List<Author> authors, string publisher, string released, double price)
        => new(Guid.NewGuid(), title, edition, publisher, released, price) { Authors = authors };

    public void AddAuthors(params Author[] authors) => Authors.AddRange(authors);
}

