using System.Text.Json.Serialization;

namespace App.Endpoints.Models;

public record struct Act(string Rel, string Href, Act.Methods Method)
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum Methods
    {
        GET, POST, PUT, PATCH, DELETE
    }

    public Act() : this("", "", Methods.GET) => throw new InvalidOperationException();
}
