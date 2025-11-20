using SmartTrade.Infrastructure.Configuration;
using SmartTrade.Application.Interfaces;
using SmartTrade.Application.Services;
using SmartTrade.Core.Domain.Interfaces;
using SmartTrade.Core.Domain.Entities;
using SmartTrade.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using SmartTrade.Infrastructure.Repositories;
using SmartTrade.Infrastructure.ExternalServices;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

namespace SmartTrade.Infrastructure.Extensions;

/// <summary>
/// Extension methods for configuring application services
/// Keeps Program.cs clean and organized
/// </summary>
public static class ServiceExtensions
{
    /// <summary>
    /// Registers all application services with dependency injection
    /// </summary>
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Database
        services.AddDbContext<StockMarketDbContext>(options => 
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
        
        // Note: Identity configuration is in Program.cs (requires web framework references)
        
        // Configuration
        services.Configure<TradingOptions>(configuration.GetSection("TradingOptions"));
        
        // HTTP Client
        services.AddHttpClient();
        
        // Application Services
        services.AddScoped<IFinnhubService, FinnhubService>();
        services.AddScoped<IStockDataMapperService, StockDataMapperService>();
        services.AddScoped<IBuyOrderService, StocksService>();
        services.AddScoped<ISellOrderService, StocksService>();
        
        // Repositories
        services.AddScoped<IStocksRepository, StocksRepository>();
        services.AddScoped<IFinnhubRepository, FinnhubRepository>();
        
        // Note: Filters are registered in Web project (they depend on controllers)
        
        return services;
    }
}

