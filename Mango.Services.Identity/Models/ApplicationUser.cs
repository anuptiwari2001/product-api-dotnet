using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Mango.Services.Identity.Models;

public sealed class ApplicationUser : IdentityUser
{
    [Required, StringLength(100)]
    public string FirstName { get; set; } = string.Empty;

    [Required, StringLength(100)]
    public string LastName { get; set; } = string.Empty;
    public string Name { get; internal set; }
}
