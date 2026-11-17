using Duende.IdentityServer;
using Duende.IdentityServer.Models;

namespace Mango.Services.Identity
{
    public static class SD
    {
        public const string Admin = "Admin";
        public const string Customer = "Customer";

        public static IEnumerable<IdentityResource> IdentityResources =>
            new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Email(),
                new IdentityResources.Profile()
            };

        public static IEnumerable<ApiScope> ApiScopes =>
            new List<ApiScope>
            {
                new ApiScope("mango", "Mango Server", new[] { "role" }),
                new ApiScope("read", "Read your data."),
                new ApiScope("write", "Write your data."),
                new ApiScope("delete", "Delete your data.")
            };

        public static IEnumerable<Client> Clients(string serviceClientSecret)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(serviceClientSecret);

            return new List<Client>
            {
                new Client
                {
                    ClientId = "client",
                    ClientSecrets = { new Secret(serviceClientSecret.Sha256()) },
                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    AllowedScopes = { "read", "write" }
                },
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
                },
                new Client
                {
                    ClientId = "swagger",
                    ClientName = "Mango Product API Swagger",
                    AllowedGrantTypes = GrantTypes.Code,
                    RequirePkce = true,
                    RequireClientSecret = false,
                    RequireConsent = false,
                    RedirectUris = { "https://localhost:7132/swagger/oauth2-redirect.html" },
                    AllowedCorsOrigins = { "https://localhost:7132" },
                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Email,
                        "mango"
                    }
                }
            };
        }
    }
}
