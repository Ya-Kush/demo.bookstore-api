namespace App.Endpoints.Models;

public readonly record struct Set<T>(IEnumerable<T> Data);
public readonly record struct LinkedItem<T>(T Item, Link[] Links);
public readonly record struct LinkedSet<T>(IEnumerable<LinkedItem<T>> Data);

public static class Set
{
    public static Set<T> New<T>(IEnumerable<T> values) => new(values);
    public static LinkedSet<T> NewLinked<T>(IEnumerable<(T, Link[])> values) => new(values.Select(v => new LinkedItem<T>(v.Item1, v.Item2)));
}

public static class Items
{
    public static IEnumerable<LinkedItem<T>> NewLinked<T>(IEnumerable<(T, Link[])> values) => values.Select(v => new LinkedItem<T>(v.Item1, v.Item2));
    public static LinkedItem<T> NewLinked<T>(T item, Link[] links) => new(item, links);
}
