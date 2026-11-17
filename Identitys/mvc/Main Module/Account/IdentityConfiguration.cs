using Duende.IdentityServer;
using Duende.IdentityServer.Models;

namespace Mango.Services.Identity.MainModule.Account;

public static class IdentityConfiguration
{
    public static IEnumerable<IdentityResource> IdentityResources =>
    [
        new IdentityResources.OpenId(),
        new IdentityResources.Profile(),
        new IdentityResources.Email()
    ];

    public static IEnumerable<ApiScope> ApiScopes =>
    [
        new ApiScope("mango", "Mango API")
    ];

    public static IEnumerable<Client> Clients =>
    [
        new Client
        {
            ClientId = "mango",
            ClientName = "Mango Web",
            AllowedGrantTypes = GrantTypes.Code,
            RequirePkce = true,
            RequireClientSecret = false,
            RequireConsent = false,
            RedirectUris = { "https://localhost:7063/signin-oidc" },
            PostLogoutRedirectUris = { "https://localhost:7063/signout-callback-oidc" },
            AllowedScopes =
            {
                IdentityServerConstants.StandardScopes.OpenId,
                IdentityServerConstants.StandardScopes.Profile,
                IdentityServerConstants.StandardScopes.Email,
                "mango"
            },
            AllowAccessTokensViaBrowser = false
        }
    ];
}
