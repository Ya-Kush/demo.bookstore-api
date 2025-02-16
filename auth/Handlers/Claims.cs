using System.Security.Claims;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Auth.Handlers;

public static class Claims
{
    public record ClaimsResponse(FlatClaim[] Claims, FlatClaimIdentity[] Identities, FlatIdentity? Identity);

    public static Ok<ClaimsResponse> Get(ClaimsPrincipal principal)
    {
        var res = new ClaimsResponse(
            Claims: [..principal.Claims.Select(ToFlat)],
            Identities: [..principal.Identities.Select(ToFlat).Where(x => x is not null)!],
            Identity: principal.Identity is null ? null : new (
                principal.Identity.AuthenticationType,
                principal.Identity.Name,
                principal.Identity.IsAuthenticated)
        );
        return TypedResults.Ok(res);
    }

    public record FlatClaim(
        string Issuer,
        string OriginalIssuer,
        IDictionary<string, string> Properties,
        string Type,
        string Value,
        string ValueType);

    public record FlatClaimIdentity(
        FlatClaimIdentity? Actor,
        string? AuthenticationType,
        object? BootstrapContext,
        bool IsAuthenticated,
        string? Label,
        string? Name,
        string NameClaimType,
        string RoleClaimType);

    public record FlatIdentity(
        string? AuthenticationType,
        string? Name,
        bool IsAuthenticated);

    static FlatClaim ToFlat(Claim claim)
        => new(
            claim.Issuer,
            claim.OriginalIssuer,
            claim.Properties,
            claim.Type,
            claim.Value,
            claim.ValueType);
    
    static FlatClaimIdentity? ToFlat(ClaimsIdentity? claimsIdentity)
        => claimsIdentity is null ? null : new(
            ToFlat(claimsIdentity.Actor),
            claimsIdentity.AuthenticationType,
            claimsIdentity.BootstrapContext,
            claimsIdentity.IsAuthenticated,
            claimsIdentity.Label,
            claimsIdentity.Name,
            claimsIdentity.NameClaimType,
            claimsIdentity.RoleClaimType);
}