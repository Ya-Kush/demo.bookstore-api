using Microsoft.EntityFrameworkCore;

namespace App.Data.Extensions;

public static class BookstoreDbContextExtensions
{
    public static IQueryable<T> Untrack<T>(this IQueryable<T> entities) where T : class
        => entities.AsNoTracking();
}
