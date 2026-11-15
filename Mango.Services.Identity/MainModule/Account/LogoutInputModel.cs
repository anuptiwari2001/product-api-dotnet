namespace Mango.Services.Identity.MainModule.Account;

public sealed class LogoutInputModel
{
    public string? LogoutId { get; set; }
    public string? ReturnUrl { get; set; }
}
