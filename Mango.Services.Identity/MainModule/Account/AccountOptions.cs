namespace Mango.Services.Identity.MainModule.Account;

public sealed class AccountOptions
{
    public bool AllowLocalLogin { get; set; } = true;
    public bool AllowRememberLogin { get; set; } = true;
    public bool AllowRegistration { get; set; } = true;
    public bool ShowLogoutPrompt { get; set; } = true;
    public TimeSpan RememberMeLoginDuration { get; set; } = TimeSpan.FromDays(30);
}
