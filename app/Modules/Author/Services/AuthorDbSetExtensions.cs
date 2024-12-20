using App.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace App.Data.Extensions;

public static class AuthorDbSetExtensions
{
    public static IQueryable<Author> WithBooks(this IQueryable<Author> authors) => authors.Include(a => a.Books);
}
