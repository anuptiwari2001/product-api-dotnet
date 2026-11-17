using System.Security.Claims;
using Mango.Services.Identity.Models;
using Microsoft.AspNetCore.Identity;

namespace Mango.Services.Identity.Initializer
{
    public sealed class DbInitializer : IDbInitializer
    {
        private const string NameClaimType = "name";
        private const string GivenNameClaimType = "given_name";
        private const string FamilyNameClaimType = "family_name";
        private const string RoleClaimType = "role";

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;

        public DbInitializer(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
        }

        public async Task InitializeAsync()
        {
            await EnsureRoleAsync(SD.Admin);
            await EnsureRoleAsync(SD.Customer);

            var adminPassword = GetRequiredSeedPassword("AdminPassword");
            var customerPassword = GetRequiredSeedPassword("CustomerPassword");

            await SeedUserAsync(
                email: "admin@gmail.com",
                password: adminPassword,
                firstName: "Anupam",
                lastName: "Tiwari",
                phoneNumber: "1234567890",
                role: SD.Admin);

            await SeedUserAsync(
                email: "customer1@gmail.com",
                password: customerPassword,
                firstName: "Ben",
                lastName: "Cust",
                phoneNumber: "11111111111",
                role: SD.Customer);
        }

        private string GetRequiredSeedPassword(string name) =>
            _configuration[$"SeedUsers:{name}"]
            ?? throw new InvalidOperationException(
                $"Configure SeedUsers:{name} in User Secrets or an environment variable for development seeding.");

        private async Task EnsureRoleAsync(string roleName)
        {
            if (await _roleManager.RoleExistsAsync(roleName))
            {
                return;
            }

            var result = await _roleManager.CreateAsync(new IdentityRole(roleName));
            EnsureSucceeded(result, $"create the {roleName} role");
        }

        private async Task SeedUserAsync(
            string email,
            string password,
            string firstName,
            string lastName,
            string phoneNumber,
            string role)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user is null)
            {
                user = new ApplicationUser
                {
                    UserName = email,
                    Email = email,
                    EmailConfirmed = true,
                    PhoneNumber = phoneNumber,
                    FirstName = firstName,
                    LastName = lastName,
                    Name = $"{firstName} {lastName}"
                };

                var createResult = await _userManager.CreateAsync(user, password);
                EnsureSucceeded(createResult, $"create the {role} seed user");
            }
            else
            {
                var changed = false;
                if (string.IsNullOrWhiteSpace(user.FirstName))
                {
                    user.FirstName = firstName;
                    changed = true;
                }

                if (string.IsNullOrWhiteSpace(user.LastName))
                {
                    user.LastName = lastName;
                    changed = true;
                }

                if (string.IsNullOrWhiteSpace(user.PhoneNumber))
                {
                    user.PhoneNumber = phoneNumber;
                    changed = true;
                }

                if (string.IsNullOrWhiteSpace(user.Name))
                {
                    user.Name = $"{user.FirstName} {user.LastName}";
                    changed = true;
                }

                if (changed)
                {
                    var updateResult = await _userManager.UpdateAsync(user);
                    EnsureSucceeded(updateResult, $"update the {role} seed user");
                }
            }

            if (!await _userManager.IsInRoleAsync(user, role))
            {
                var roleResult = await _userManager.AddToRoleAsync(user, role);
                EnsureSucceeded(roleResult, $"assign the {role} role to its seed user");
            }

            var claims = new[]
            {
                new Claim(NameClaimType, $"{user.FirstName} {user.LastName}"),
                new Claim(GivenNameClaimType, user.FirstName),
                new Claim(FamilyNameClaimType, user.LastName),
                new Claim(RoleClaimType, role)
            };
            var existingClaims = await _userManager.GetClaimsAsync(user);
            var missingClaims = claims
                .Where(claim => !existingClaims.Any(existing =>
                    existing.Type == claim.Type && existing.Value == claim.Value))
                .ToArray();

            if (missingClaims.Length > 0)
            {
                var claimsResult = await _userManager.AddClaimsAsync(user, missingClaims);
                EnsureSucceeded(claimsResult, $"add profile claims to the {role} seed user");
            }
        }

        private static void EnsureSucceeded(IdentityResult result, string operation)
        {
            if (!result.Succeeded)
            {
                var errors = string.Join("; ", result.Errors.Select(error => error.Description));
                throw new InvalidOperationException($"Could not {operation}: {errors}");
            }
        }
    }
}
