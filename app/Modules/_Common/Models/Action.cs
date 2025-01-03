using System.Text.Json.Serialization;

namespace App.Endpoints.Models;

public record struct Action(string Rel, string Href, Action.Methods Method)
{
    public static class HttpMethod
    {
        public const string GET = nameof(GET);
        public const string POST = nameof(POST);
        public const string PUT = nameof(PUT);
        public const string PATCH = nameof(PATCH);
        public const string DELETE = nameof(DELETE);
    }
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum Methods
    {
        GET, POST, PUT, PATCH, DELETE
    }

    public Action() : this("", "", Methods.GET) => throw new InvalidOperationException();
}
