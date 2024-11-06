using System.Diagnostics.CodeAnalysis;

namespace App.Models;

public readonly struct Guid<T>(Guid val)
{
    public readonly Guid Value { get; } = val;
    public readonly static Guid<T> Empty = new(Guid.Empty);

    public override bool Equals([NotNullWhen(true)] object? obj) => Value.Equals(obj);
    public override int GetHashCode() => Value.GetHashCode();
    public override string ToString() => $"{typeof(T).Name}:{base.ToString()}";

    #warning Not sure that is the right way
    public static bool operator ==(Guid<T> tGuid, Guid guid) => tGuid.Equals(guid);
    public static bool operator !=(Guid<T> tGuid, Guid guid) => tGuid.Equals(guid) is false;
}
