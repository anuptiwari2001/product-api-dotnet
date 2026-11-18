using Mango.Web;
using Mango.Web.Services;
using Mango.Web.Services.iServices;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var services = builder.Services;
services.AddHttpClient("MangoAPI", client =>
{
    client.Timeout = TimeSpan.FromSeconds(10);
})
.ConfigurePrimaryHttpMessageHandler(() => new SocketsHttpHandler
{
    // Local API calls must not go through a system proxy, or Products sits and waits.
    UseProxy = false,
    ConnectTimeout = TimeSpan.FromSeconds(3)
});
services.AddScoped<IProductService, ProductService>();
SD.ProductAPIBase = builder.Configuration["ServiceUrls:ProductAPI"] ?? SD.ProductAPIBase;

var identityAuthority = builder.Configuration["ServiceUrls:IdentityAPI"];
if (string.IsNullOrWhiteSpace(identityAuthority))
{
    throw new InvalidOperationException("ServiceUrls:IdentityAPI must be configured.");
}

services.AddAuthentication(options =>
{
    options.DefaultScheme = "Cookies";
    options.DefaultChallengeScheme = "oidc";
})
.AddCookie("Cookies", options =>
{
    options.ExpireTimeSpan = TimeSpan.FromMinutes(10);
})
.AddOpenIdConnect("oidc", options =>
{
    options.Authority = identityAuthority.TrimEnd('/');
    options.ClientId = "mango";
    options.ResponseType = OpenIdConnectResponseType.Code;
    options.UsePkce = true;
    options.SaveTokens = true;
    options.GetClaimsFromUserInfoEndpoint = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        NameClaimType = "name",
        RoleClaimType = "role"
    };
    options.Scope.Add("profile");
    options.Scope.Add("email");
    options.Scope.Add("mango");
});
services.AddAuthorization();
services.AddControllersWithViews().AddRazorRuntimeCompilation();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
