namespace Mango.Services.Identity.MainModule.Account;

public sealed class LoggedOutViewModel
{
    public string? ClientName { get; set; }
    public string? PostLogoutRedirectUri { get; set; }
    public string? SignOutIframeUrl { get; set; }
}
