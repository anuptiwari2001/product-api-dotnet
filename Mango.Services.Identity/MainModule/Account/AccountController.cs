using Duende.IdentityServer.Services;
using Mango.Services.Identity.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Mango.Services.Identity.MainModule.Account;

[AllowAnonymous]
[Route("[controller]/[action]")]
public sealed class AccountController(
    UserManager<ApplicationUser> userManager,
    SignInManager<ApplicationUser> signInManager,
    IIdentityServerInteractionService interactionService,
    IOptions<AccountOptions> accountOptions) : Controller
{
    private AccountOptions Options => accountOptions.Value;

    [HttpGet]
    public async Task<IActionResult> Login(string? returnUrl, string? error) =>
        View(await BuildLoginViewModelAsync(returnUrl, error: error));

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginInputModel input)
    {
        input.ReturnUrl = GetValidReturnUrl(input.ReturnUrl);
        if (!Options.AllowLocalLogin)
        {
            ModelState.AddModelError(string.Empty, "Local sign-in is disabled.");
        }

        if (!ModelState.IsValid)
        {
            return View(await BuildLoginViewModelAsync(input.ReturnUrl, input));
        }

        var result = await signInManager.PasswordSignInAsync(
            input.Email, input.Password, input.RememberMe && Options.AllowRememberLogin, lockoutOnFailure: true);

        if (result.Succeeded)
        {
            return RedirectToLocal(input.ReturnUrl);
        }

        var error = result.IsLockedOut
            ? "This account is temporarily locked. Try again later."
            : "Invalid email or password.";
        return View(await BuildLoginViewModelAsync(input.ReturnUrl, input, error));
    }

    [HttpGet]
    public IActionResult Register(string? returnUrl)
    {
        if (!Options.AllowRegistration)
        {
            return NotFound();
        }

        return View(new RegisterViewModel { ReturnUrl = GetValidReturnUrl(returnUrl) });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel input)
    {
        input.ReturnUrl = GetValidReturnUrl(input.ReturnUrl);
        if (!Options.AllowRegistration)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            return View(input);
        }

        var user = new ApplicationUser
        {
            UserName = input.Email,
            Email = input.Email,
            FirstName = input.FirstName,
            LastName = input.LastName
        };

        var result = await userManager.CreateAsync(user, input.Password);
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(input);
        }

        await signInManager.SignInAsync(user, isPersistent: false);
        return RedirectToLocal(input.ReturnUrl);
    }

    [HttpGet]
    public async Task<IActionResult> Logout(string? logoutId, string? returnUrl)
    {
        var context = await interactionService.GetLogoutContextAsync(logoutId, HttpContext.RequestAborted);
        return View(new LogoutViewModel
        {
            LogoutId = logoutId,
            ClientName = context?.ClientName,
            PostLogoutRedirectUri = context?.PostLogoutRedirectUri ?? GetValidReturnUrl(returnUrl),
            SignOutIframeUrl = context?.SignOutIFrameUrl,
            ShowSignoutPrompt = User.Identity?.IsAuthenticated == true && Options.ShowLogoutPrompt
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout(LogoutInputModel input)
    {
        var context = await interactionService.GetLogoutContextAsync(input.LogoutId, HttpContext.RequestAborted);
        if (User.Identity?.IsAuthenticated == true)
        {
            await signInManager.SignOutAsync();
        }

        var returnUrl = context?.PostLogoutRedirectUri ?? GetValidReturnUrl(input.ReturnUrl);
        if (string.IsNullOrEmpty(returnUrl) && string.IsNullOrEmpty(context?.SignOutIFrameUrl))
        {
            return RedirectToAction("Index", "Home");
        }

        return View("LoggedOut", new LoggedOutViewModel
        {
            ClientName = context?.ClientName,
            PostLogoutRedirectUri = returnUrl,
            SignOutIframeUrl = context?.SignOutIFrameUrl
        });
    }

    private async Task<LoginViewModel> BuildLoginViewModelAsync(
        string? returnUrl, LoginInputModel? input = null, string? error = null)
    {
        var externalProviders = await signInManager.GetExternalAuthenticationSchemesAsync();
        return new LoginViewModel
        {
            Input = input ?? new LoginInputModel { ReturnUrl = GetValidReturnUrl(returnUrl) },
            EnableLocalLogin = Options.AllowLocalLogin,
            AllowRememberLogin = Options.AllowRememberLogin,
            AllowRegistration = Options.AllowRegistration,
            ErrorMessage = error,
            ExternalProviders = externalProviders
                .Where(provider => !string.IsNullOrWhiteSpace(provider.DisplayName))
                .Select(provider => new ExternalProvider(provider.DisplayName!, provider.Name))
                .ToArray()
        };
    }

    private string? GetValidReturnUrl(string? returnUrl) =>
        !string.IsNullOrWhiteSpace(returnUrl) &&
        (Url.IsLocalUrl(returnUrl) || interactionService.IsValidReturnUrl(returnUrl))
            ? returnUrl
            : null;

    private IActionResult RedirectToLocal(string? returnUrl) =>
        !string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl)
            ? Redirect(returnUrl)
            : RedirectToAction("Index", "Home");
}
