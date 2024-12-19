using App.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace App.Data.Extensions;

public static class DbContextBookExtensions
{
    public static IQueryable<Book> WithAuthors(this IQueryable<Book> books) => books.Include(b => b.Authors);
}
