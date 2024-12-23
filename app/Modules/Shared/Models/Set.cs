namespace App.Endpoints.Models;

public readonly record struct Set<T>(IEnumerable<T> Data);

public static class Set
{
    public static Set<T> New<T>(IEnumerable<T> values) => new(values);
}
