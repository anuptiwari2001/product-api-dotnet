# Mango.Services.ProductAPI

A .NET 10 ASP.NET Core Web API for reading product data. It uses Entity Framework Core with SQL Server, AutoMapper, and Swagger/OpenAPI.

## Features

- List products
- Retrieve a product by ID
- Entity Framework Core SQL Server persistence
- Swagger UI for API exploration

> The current API is read-only. Create, update, and delete HTTP endpoints are not implemented.

## API Endpoints

| Method | Endpoint | Description |
| --- | --- | --- |
| `GET` | `/api/products` | Returns the product list. |
| `GET` | `/api/products/{id}` | Returns a product by ID, or `404` if it does not exist. |

Product records include `ProductId`, `Name`, `Description`, `CategoryName`, `Price`, and `ImageUrl`.

## Requirements

- .NET 10 SDK
- SQL Server

## Configuration

Configure the `ConnectionStrings:DefaultConnection` setting for a SQL Server instance that is available to your environment. Do not commit credentials or other secrets. You can override the setting using the `ConnectionStrings__DefaultConnection` environment variable.

For example, in PowerShell:

```powershell
$env:ConnectionStrings__DefaultConnection = "Server=localhost;Database=MangoProductAPI;Trusted_Connection=True;TrustServerCertificate=True"
```

## Run Locally

From the project directory:

```powershell
dotnet restore
dotnet ef database update
dotnet run --launch-profile https
```

If the `dotnet ef` command is unavailable, install the EF Core CLI tool:

```powershell
dotnet tool install --global dotnet-ef
```

The HTTPS launch profile uses `https://localhost:7132`. In Development, Swagger UI is available at [https://localhost:7132/swagger](https://localhost:7132/swagger).
