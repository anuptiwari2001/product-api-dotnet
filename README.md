# Mango Restaurant

Mango Restaurant is a .NET 10 solution containing an ASP.NET Core MVC web application and a product management API.

## Projects

- **Mango.Web** — MVC frontend. Runs at `https://localhost:7063` (or `http://localhost:5083`).
- **Mango.Services.ProductAPI** — REST API backed by SQL Server. Runs at `https://localhost:7132` (or `http://localhost:5095`). Swagger UI is available at `https://localhost:7132/swagger` in Development.

## Requirements

- .NET 10 SDK
- SQL Server

## Configuration

Set the API's `ConnectionStrings:DefaultConnection` to a SQL Server connection string for your environment. The API project includes a local development default. For a different database, set the `ConnectionStrings__DefaultConnection` environment variable rather than committing credentials.

The web project reads the API URL from `Mango.Web/appsettings.json` under `ServiceUrls:ProductAPI`.

## Build

From the solution root:

```powershell
dotnet restore MangoRestaurant.slnx
dotnet build MangoRestaurant.slnx
```

## Run locally

Start the API in one terminal:

```powershell
dotnet run --project Mango.Services.ProductAPI/Mango.Services.ProductAPI.csproj --launch-profile https
```

Start the web application in a second terminal:

```powershell
dotnet run --project Mango.Web/Mango.Web.csproj --launch-profile https
```

Open the web application at [https://localhost:7063](https://localhost:7063) and Swagger at [https://localhost:7132/swagger](https://localhost:7132/swagger). Trust the ASP.NET Core HTTPS development certificate if your browser reports that it is not trusted.

## Product API endpoints

| Method | Endpoint | Description |
| --- | --- | --- |
| `GET` | `/api/products` | List products. |
| `GET` | `/api/products/{id}` | Get a product by ID. |
| `POST` | `/api/products` | Create a product. |
| `PUT` | `/api/products` | Update a product. |
| `DELETE` | `/api/products/{id}` | Delete a product. |
