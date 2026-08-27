# OrderProcessingApi

A production-ready REST API for real-time inventory and order processing built with ASP.NET Core and Entity Framework Core.

## Features

- Product management
- Order processing
- Stock availability validation
- Automatic stock deduction
- Transaction rollback
- Optimistic concurrency handling
- Global exception handling
- Dependency Injection
- Async/Await
- Automated unit tests

## Tech Stack

- .NET / ASP.NET Core Web API
- C#
- Entity Framework Core
- SQLite
- xUnit
- Docker
- Swagger

## API Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/products` | Get all products |
| POST | `/api/products` | Create a product |
| POST | `/api/orders` | Create an order |
| GET | `/api/orders/{id}` | Get order details |

## Testing

Tests cover:

- Successful order and stock deduction
- Insufficient stock validation
- Multi-item transaction rollback

Run tests:

```bash
dotnet test

Run Locally

```bash
dotnet restore
dotnet run
