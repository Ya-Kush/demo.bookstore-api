using Microsoft.EntityFrameworkCore;

namespace App.Data;

public static class DbContextExtensions
{
    public static IQueryable<T> Untrack<T>(this IQueryable<T> values) where T : class
        => values.AsNoTracking();
}
