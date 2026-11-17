using Duende.IdentityServer.Services;
using Mango.Services.Identity.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Mango.Services.Identity.MainModule.Account;

[AllowAnonymous]
[Route("[controller]/[action]")]
public sealed class ExternalController(
    SignInManager<ApplicationUser> signInManager,
    IIdentityServerInteractionService interactionService) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Challenge(string authenticationScheme, string? returnUrl)
    {
        var providers = await signInManager.GetExternalAuthenticationSchemesAsync();
        var provider = providers.FirstOrDefault(item => item.Name == authenticationScheme);
        if (provider is null)
        {
            return BadRequest("The requested external sign-in provider is not configured.");
        }

        var properties = signInManager.ConfigureExternalAuthenticationProperties(
            authenticationScheme,
            Url.Action(nameof(Callback), new { returnUrl = GetValidReturnUrl(returnUrl) })!);
        return new ChallengeResult(authenticationScheme, properties);
    }

    [HttpGet]
    public async Task<IActionResult> Callback(string? returnUrl, string? remoteError)
    {
        if (!string.IsNullOrWhiteSpace(remoteError))
        {
            return LoginWithError("External sign-in failed. Please try again.", returnUrl);
        }

        var info = await signInManager.GetExternalLoginInfoAsync();
        if (info is null)
        {
            return LoginWithError("External sign-in information could not be loaded.", returnUrl);
        }

        var result = await signInManager.ExternalLoginSignInAsync(
            info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);
        if (result.Succeeded)
        {
            return RedirectToLocal(returnUrl);
        }

        var message = result.IsLockedOut
            ? "This account is temporarily locked. Try again later."
            : "This external account is not linked to a Mango account. Sign in with your email and link the provider in account settings.";
        return LoginWithError(message, returnUrl);
    }

    private string? GetValidReturnUrl(string? returnUrl) =>
        !string.IsNullOrWhiteSpace(returnUrl) &&
        (Url.IsLocalUrl(returnUrl) || interactionService.IsValidReturnUrl(returnUrl))
            ? returnUrl
            : null;

    private IActionResult LoginWithError(string message, string? returnUrl) =>
        RedirectToAction("Login", "Account", new { returnUrl = GetValidReturnUrl(returnUrl), error = message })!;

    private IActionResult RedirectToLocal(string? returnUrl) =>
        !string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl)
            ? Redirect(returnUrl)
            : RedirectToAction("Index", "Home");
}
