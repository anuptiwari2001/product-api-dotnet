using System.Security.Cryptography.X509Certificates;
using Duende.IdentityServer;
using Mango.Services.Identity;
using Mango.Services.Identity.DbContexts;
using Mango.Services.Identity.MainModule.Account;
using Mango.Services.Identity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException("ConnectionStrings:DefaultConnection must be configured.");
}

builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString));
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
    {
        options.Lockout.AllowedForNewUsers = true;
        options.Lockout.MaxFailedAccessAttempts = 5;
        options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(10);
        options.Password.RequiredLength = 8;
        options.Password.RequireDigit = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireUppercase = true;
        options.Password.RequireNonAlphanumeric = true;
    })
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

var serviceClientSecret = builder.Configuration["IdentityServer:Clients:ServiceSecret"];
if (string.IsNullOrWhiteSpace(serviceClientSecret))
{
    throw new InvalidOperationException(
        "Configure IdentityServer:Clients:ServiceSecret with the client-credentials secret.");
}

var identityServer = builder.Services.AddIdentityServer(options =>
{
    options.Events.RaiseErrorEvents = true;
    options.Events.RaiseFailureEvents = true;
    options.Events.RaiseInformationEvents = true;
    options.Events.RaiseSuccessEvents = true;
    options.EmitStaticAudienceClaim = true;
})
.AddInMemoryIdentityResources(SD.IdentityResources)
.AddInMemoryApiScopes(SD.ApiScopes)
.AddInMemoryClients(SD.Clients(serviceClientSecret))
.AddAspNetIdentity<ApplicationUser>();

if (builder.Environment.IsDevelopment())
{
    identityServer.AddDeveloperSigningCredential();
}
else
{
    var certificatePath = builder.Configuration["IdentityServer:SigningCertificate:Path"];
    if (string.IsNullOrWhiteSpace(certificatePath))
    {
        throw new InvalidOperationException(
            "Configure IdentityServer:SigningCertificate:Path with a production signing certificate.");
    }

    var certificatePassword = builder.Configuration["IdentityServer:SigningCertificate:Password"];
    var certificate = X509CertificateLoader.LoadPkcs12FromFile(certificatePath, certificatePassword);
    if (!certificate.HasPrivateKey)
    {
        throw new InvalidOperationException("The configured IdentityServer signing certificate must include a private key.");
    }

    identityServer.AddSigningCredential(certificate);
}

builder.Services.Configure<AccountOptions>(builder.Configuration.GetSection("Account"));
builder.Services.ConfigureApplicationCookie(options =>
{
    options.ExpireTimeSpan = builder.Configuration.GetValue<TimeSpan?>("Account:RememberMeLoginDuration")
        ?? TimeSpan.FromDays(30);
    options.SlidingExpiration = true;
});
builder.Services.AddControllersWithViews();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var database = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    database.Database.Migrate();
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseIdentityServer();
app.UseAuthorization();
app.MapStaticAssets();
app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
