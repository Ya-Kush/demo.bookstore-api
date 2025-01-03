using System.Text.Json.Serialization;

namespace App.Endpoints.Models;

public record struct Link(string Rel, string Href)
{
    public Link() : this("", "") => throw new InvalidOperationException();
}
