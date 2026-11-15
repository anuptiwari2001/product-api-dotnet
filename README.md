# Mango Restaurant

Mango Restaurant is a .NET 10 solution containing an ASP.NET Core MVC web application, identity service, and product management API.

## Projects

- **Mango.Web** — MVC frontend. Runs at `https://localhost:7063` (or `http://localhost:5083`).
- **Mango.Services.Identity** — Duende IdentityServer and ASP.NET Identity account service. Runs at `https://localhost:7187` (or `http://localhost:5193`).
- **Mango.Services.ProductAPI** — REST API backed by SQL Server. Runs at `https://localhost:7132` (or `http://localhost:5095`). Swagger UI is available at `https://localhost:7132/swagger` in Development.

## Requirements

- .NET 10 SDK
- SQL Server

## Configuration

Set each service's `ConnectionStrings:DefaultConnection` to a SQL Server connection string for your environment. Both services include local development defaults. Override a setting with the `ConnectionStrings__DefaultConnection` environment variable rather than committing credentials. The identity service creates its ASP.NET Identity schema on first run.

The web project reads the API URL from `Mango.Web/appsettings.json` under `ServiceUrls:ProductAPI`. For production, configure `IdentityServer:SigningCertificate:Path` and `IdentityServer:SigningCertificate:Password` using environment variables or a secret store; the developer signing credential is used only in Development.

## Build

From the solution root:

```powershell
dotnet restore MangoRestaurant.slnx
dotnet build MangoRestaurant.slnx
```

## Run locally

### Visual Studio

Select the **Mango Full Stack** launch profile from the solution's startup-profile dropdown, then press **F5**. This starts the identity service, Product API, and web application. If the profile isn't available, enable multi-project launch profiles in Visual Studio's Preview Features settings and reopen the solution.

### Command line

Start the identity service, API, and web application in separate terminals:

```powershell
dotnet run --project Mango.Services.Identity/Mango.Services.Identity.csproj --launch-profile https
dotnet run --project Mango.Web/Mango.Web.csproj --launch-profile https
dotnet run --project Mango.Services.ProductAPI/Mango.Services.ProductAPI.csproj --launch-profile https
```

Open the identity service at [https://localhost:7187](https://localhost:7187), the web application at [https://localhost:7063](https://localhost:7063), and Swagger at [https://localhost:7132/swagger](https://localhost:7132/swagger). Trust the ASP.NET Core HTTPS development certificate if your browser reports that it is not trusted.

## Product API endpoints

| Method | Endpoint | Description |
| --- | --- | --- |
| `GET` | `/api/products` | List products. |
| `GET` | `/api/products/{id}` | Get a product by ID. |
| `POST` | `/api/products` | Create a product. |
| `PUT` | `/api/products` | Update a product. |
| `DELETE` | `/api/products/{id}` | Delete a product. |
