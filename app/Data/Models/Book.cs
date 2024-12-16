namespace App.Data.Models;

public sealed class Book : IBookstoreModel
{
    public Guid Id { get; }
    public string Title { get; set; }
    public string Edition { get; set; }
    public double Price { get; set; }

    readonly List<Author> _authors = [];
    public IEnumerable<Author> Authors => _authors;

    internal Book(Guid id, string title, string edition, double price)
        => (Id, Title, Edition, Price) = (id, title, edition, price);

    public static Book New(string title, string edition, double price)
        => new(Guid.NewGuid(), title, edition, price);

    public Book WithAuthors(IEnumerable<Author> authors)
    {
        AddAuthors(authors);
        return this;
    }

    public bool HasAuthor(Author author) => _authors.Any(a => a.Id == author.Id);
    public bool HasAuthor(Guid id) => _authors.Any(a => a.Id == id);
    public bool HasEachOther(Author author)
    {
        var (ha, hb) = (HasAuthor(author), author.HasBook(this));
        if (ha ^ hb) throw new InvalidOperationException(message: $"desync data in {this} and {author} objects");
        return ha && hb;
    }


    public void AddAuthors(IEnumerable<Author> authors)
    {
        foreach (var a in authors) AddAuthor(a);
    }
    public void AddAuthor(Author author)
    {
        if (_authors.Any(x => x.Id == author.Id))

        _authors.Add(author);
        author.AddBook(this);
    }

    public void RemoveAuthors(IEnumerable<Author> authors)
    {
        foreach (var a in authors) RemoveAuthor(a);
    }
    public void RemoveAuthor(Author author)
    {
        if (_authors.All(x => x.Id != author.Id)) return;

        _authors.Remove(author);
        author.RemoveBook(this);
    }
}
