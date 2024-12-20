using Microsoft.EntityFrameworkCore;

namespace App.Data;

public static class BookstoreDbContextExtensions
{
    public static IQueryable<T> Untrack<T>(this IQueryable<T> values) where T : class
        => values.AsNoTracking();
}
