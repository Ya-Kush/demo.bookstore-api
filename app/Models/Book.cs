namespace App.Models;

public sealed class Book
{
    public Guid<Book> Id { get; }
    public string Title { get; set; }
    public string Edition { get; set; }
    public string[] Authors { get; private set; }
    public string Publisher { get; set; }
    public string Released { get; set; }
    public double Price { get; set; }

    private Book(Guid<Book> id, string title, string edition, string[] authors, string publisher, string released, double price) =>
        (Id, Title, Edition, Authors, Publisher, Released, Price) = (id, title, edition, authors, publisher, released, price);

    public static Book New(string title, string edition, string[] authors, string publisher, string released, double price) =>
        new(new(Guid.NewGuid()), title, edition, authors, publisher, released, price);

}
