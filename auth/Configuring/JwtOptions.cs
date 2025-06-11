using System.ComponentModel.DataAnnotations;

namespace Auth.Configuring;

public sealed class JwtOptions
{
    public const string SectionName = "Jwt";

    [Required] public string Issuer { get; set; } = null!;
    [Required] public string Audience { get; set; } = null!;
    [Required] public string Key { get; set; } = null!;
}