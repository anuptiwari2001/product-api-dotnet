namespace Mango.Services.Identity.MainModule.Account;

public sealed class LoginViewModel
{
    public LoginInputModel Input { get; set; } = new();
    public bool EnableLocalLogin { get; set; } = true;
    public bool AllowRememberLogin { get; set; } = true;
    public bool AllowRegistration { get; set; } = true;
    public string? ErrorMessage { get; set; }
    public IReadOnlyCollection<ExternalProvider> ExternalProviders { get; set; } = [];
}
