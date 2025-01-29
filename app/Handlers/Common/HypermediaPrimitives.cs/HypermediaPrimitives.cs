using System.Text.Json.Serialization;

namespace App.Handlers.HypermediaPrimitives;

public readonly record struct Link(string Rel, string Href);
public readonly record struct Field(string Name, string Type);
public readonly record struct Act(string Name, Act.Methods Method, string Href, Field[] Fields)
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum Methods { GET, POST, PUT, PATCH, DELETE }
}

public interface IProps;
public readonly record struct NoneProps: IProps;
public interface IItem<P> where P : IProps
{
    string Type { get; }
    Link[] Links { get; }
    Act[] Actions { get; }
    P Props { get; }
}
public readonly record struct Item<P>(string Type, Link[] Links, Act[] Actions, P Props) : IItem<P> where P : IProps;
public readonly record struct Set<IP>(string Type, Link[] Links, Act[] Actions, IEnumerable<Item<IP>> Items) : IItem<NoneProps> where IP : IProps { public NoneProps Props => new(); }
public readonly record struct Set<P,IP>(string Type, Link[] Links, Act[] Actions, P Props, IEnumerable<Item<IP>> Items) : IItem<P> where P : IProps where IP : IProps;

public static class Set
{
    public static Set<IP> New<IP>(IEnumerable<Item<IP>> items, Link[] links, Act[] acts) where IP : IProps
    {
        return new("set", links, acts, items);
    }
    public static Set<P,IP> New<P,IP>(IEnumerable<Item<IP>> items, Link[] links, Act[] acts, P props) where P : IProps where IP : IProps
    {
        return new("set", links, acts, props, items);
    }

    public static Set<IP> New<T,IP>(IEnumerable<T> data, Func<T,Item<IP>> converter, Link[] links, Act[] acts) where IP : IProps
    {
        return new("set", links, acts, data.Select(converter));
    }
    public static Set<P,IP> New<T,P,IP>(IEnumerable<T> data, Func<T,Item<IP>> converter, Link[] links, Act[] acts, P props) where P : IProps where IP : IProps
    {
        return new("set", links, acts, props, data.Select(converter));
    }

    public static Set<IP> ToSet<T,IP>(this IEnumerable<T> data, Func<T,Item<IP>> converter, Link[] links, Act[] acts) where IP : IProps
    {
        return new("set", links, acts, data.Select(converter));
    }
    public static Set<P,IP> ToSet<T,P,IP>(this IEnumerable<T> data, Func<T,Item<IP>> converter, Link[] links, Act[] acts, P props) where P : IProps where IP : IProps
    {
        return new("set", links, acts, props, data.Select(converter));
    }
}

public static class Item
{
    public static Item<P> New<P>(Link[] links, Act[] acts, P props) where P : IProps
    {
        return new(props.GetType().Name, links, acts, props);
    }
}