# SmartTrade - Stock Trading Web Application

ASP.NET Core 9.0 stock trading web application built with **Clean Architecture**. Features real-time stock data via Finnhub API, buy/sell order management, user authentication, and PDF order reports.

## 🏗️ Clean Architecture Structure

This project follows **Clean Architecture** principles, organizing code into distinct layers with clear dependency rules. Here's how it's structured:

```
SmartTrade/
├── SmartTrade.Core/              ← Domain Layer (Innermost)
├── SmartTrade.Application/       ← Application Layer
├── SmartTrade.Infrastructure/    ← Infrastructure Layer
├── SmartTrade/                   ← Presentation Layer (Outermost)
└── SmartTrade.Tests/             ← Test Project
```

### 📐 Architecture Layers

#### 1. **SmartTrade.Core** (Domain Layer)
- **Purpose**: Contains the core business entities and domain logic
- **Dependencies**: None (pure domain, no external dependencies)
- **Contains**:
  - Domain entities (`ApplicationUser`, `ApplicationRole`, `BuyOrder`, `SellOrder`, `StockTrade`)
  - Domain interfaces (`IFinnhubRepository`, `IStocksRepository`)
- **Rule**: This layer has **NO dependencies** on other layers

#### 2. **SmartTrade.Application** (Application Layer)
- **Purpose**: Contains application business logic and use cases
- **Dependencies**: `SmartTrade.Core` only
- **Contains**:
  - Application services (`FinnhubService`, `StocksService`, `StockDataMapperService`)
  - DTOs (Data Transfer Objects) for requests/responses
  - Application interfaces (`IBuyOrderService`, `ISellOrderService`, `IFinnhubService`)
- **Rule**: Depends only on `Core`, never on `Infrastructure` or `Presentation`

#### 3. **SmartTrade.Infrastructure** (Infrastructure Layer)
- **Purpose**: Implements technical details (database, external APIs, file system)
- **Dependencies**: `SmartTrade.Core` and `SmartTrade.Application`
- **Contains**:
  - Data access (`StockMarketDbContext`, Entity Framework migrations)
  - Repository implementations (`StocksRepository`, `FinnhubRepository`)
  - External service integrations (Finnhub API client)
  - Middleware (`ExceptionHandlingMiddleware`)
  - Configuration (`TradingOptions`)
- **Rule**: Implements interfaces defined in `Core` and `Application`

#### 4. **SmartTrade** (Presentation Layer)
- **Purpose**: User interface and entry point of the application
- **Dependencies**: All layers (`Core`, `Application`, `Infrastructure`)
- **Contains**:
  - Controllers (`TradeController`, `AccountController`, `HomeController`)
  - Views (Razor pages)
  - ViewModels and presentation DTOs
  - Filters (`CreateOrderActionFilter`)
  - `Program.cs` (application startup)
- **Rule**: This is the outermost layer, depends on everything

### 🔄 Dependency Flow

```
┌─────────────────────────────────────┐
│   SmartTrade (Presentation)         │  ← Depends on all layers
│   - Controllers, Views, Program.cs  │
└──────────────┬──────────────────────┘
               │
               ▼
┌─────────────────────────────────────┐
│   SmartTrade.Infrastructure         │  ← Depends on Core & Application
│   - DbContext, Repositories, APIs   │
└──────────────┬──────────────────────┘
               │
               ▼
┌─────────────────────────────────────┐
│   SmartTrade.Application            │  ← Depends on Core only
│   - Services, Business Logic, DTOs  │
└──────────────┬──────────────────────┘
               │
               ▼
┌─────────────────────────────────────┐
│   SmartTrade.Core (Domain)          │  ← No dependencies
│   - Entities, Domain Interfaces     │
└─────────────────────────────────────┘
```

**Key Principle**: Dependencies point **inward** toward the core. The core has no dependencies, and outer layers depend on inner layers.

## 🚀 Features

- ✅ **Real-time Stock Data**: Integration with Finnhub API for live stock prices
- ✅ **Order Management**: Buy and sell stock orders with validation
- ✅ **User Authentication**: ASP.NET Core Identity with role-based access
- ✅ **PDF Reports**: Generate PDF reports of trading orders using Rotativa
- ✅ **Clean Architecture**: Separation of concerns with clear layer boundaries
- ✅ **Logging**: Comprehensive logging with Serilog
- ✅ **Exception Handling**: Global exception handling middleware
- ✅ **Unit & Integration Tests**: Test coverage for services and controllers

## 🛠️ Technology Stack

- **.NET 9.0** - Latest .NET framework
- **ASP.NET Core MVC** - Web framework
- **Entity Framework Core 9.0** - ORM for data access
- **SQL Server** - Database
- **ASP.NET Core Identity** - Authentication and authorization
- **Serilog** - Structured logging
- **Rotativa** - PDF generation
- **Finnhub API** - Stock market data

## 📦 Project Structure Details

### Main Clean Architecture Projects

| Project | Layer | Purpose |
|---------|-------|---------|
| `SmartTrade.Core` | Domain | Business entities and domain logic |
| `SmartTrade.Application` | Application | Business use cases and services |
| `SmartTrade.Infrastructure` | Infrastructure | Data access and external services |
| `SmartTrade` | Presentation | Web UI and API endpoints |

### Additional Projects

The solution also includes some additional projects that may have been created during development:

- `SmartTrade.Entities` - May contain entity definitions (consider consolidating with Core)
- `SmartTrade.Repositories` - Repository implementations (consider consolidating with Infrastructure)
- `SmartTrade.RepositoryContracts` - Repository interfaces (consider consolidating with Core)
- `SmartTrade.ServiceContracts` - Service interfaces (consider consolidating with Application)
- `SmartTrade.Services` - Service implementations (consider consolidating with Application)
- `SmartTrade.Tests` - Unit and integration tests

> **Note**: For a cleaner architecture, consider consolidating duplicate projects into the main four layers.

## 🔧 Getting Started

### Prerequisites

- .NET 9.0 SDK
- SQL Server (LocalDB or full instance)
- Visual Studio 2022 or VS Code
- Finnhub API key (get one at [finnhub.io](https://finnhub.io))

### Setup

1. **Clone the repository**
   ```bash
   git clone https://github.com/Mommeeeh1/SmartTrade.git
   cd SmartTrade
   ```

2. **Configure appsettings.json**
   - Add your Finnhub API token to `appsettings.json`:
   ```json
   "TradingOptions": {
     "FinnhubToken": "your-api-key-here"
   }
   ```

3. **Update database connection string**
   - Update the connection string in `appsettings.json` if needed

4. **Run migrations**
   ```bash
   cd SmartTrade
   dotnet ef database update --project ../SmartTrade.Infrastructure
   ```

5. **Run the application**
   ```bash
   dotnet run --project SmartTrade
   ```

6. **Access the application**
   - Navigate to `https://localhost:5001` or `http://localhost:5000`

## 📝 Clean Architecture Benefits

1. **Independence**: Business logic is independent of frameworks, UI, and databases
2. **Testability**: Easy to test business logic without external dependencies
3. **Flexibility**: Can swap out infrastructure (database, APIs) without changing business logic
4. **Maintainability**: Clear separation makes code easier to understand and maintain
5. **Scalability**: Easy to add new features without affecting existing code

## 🧪 Testing

Run tests using:
```bash
dotnet test
```

## 📄 License

This project is open source and available for learning purposes.

## 🤝 Contributing

Contributions, issues, and feature requests are welcome!

---

**Built with Clean Architecture principles** 🏛️

