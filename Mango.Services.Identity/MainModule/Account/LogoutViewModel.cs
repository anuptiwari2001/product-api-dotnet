namespace Mango.Services.Identity.MainModule.Account;

public sealed class LogoutViewModel
{
    public string? LogoutId { get; set; }
    public string? ClientName { get; set; }
    public string? PostLogoutRedirectUri { get; set; }
    public string? SignOutIframeUrl { get; set; }
    public bool ShowSignoutPrompt { get; set; }
}
