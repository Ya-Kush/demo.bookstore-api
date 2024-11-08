namespace App.DomainModels;

public sealed class Author
{
    public Guid Guid { get; }
    public string FirstName { get; set; }
    public string MiddleName { get; set; }
    public string LastName { get; set; }

    public List<Book> Books { get; init; } = [];

    private Author(Guid guid, string firstName, string middleName, string lastName)
        => (Guid, FirstName, MiddleName, LastName) = (guid, firstName, middleName, lastName);

    public static Author New(string firstName, string middleName, string lastName, List<Book> books)
        => new(Guid.NewGuid(), firstName, middleName, lastName) { Books = books };

    public void AddBooks(params Book[] books) => Books.AddRange(books);
}
