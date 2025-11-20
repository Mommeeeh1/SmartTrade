using SmartTrade.Core.Domain.Entities;

namespace SmartTrade.Application.Interfaces;

/// <summary>
/// Service responsible for mapping raw stock data to application models
/// Follows Single Responsibility Principle: Only handles data mapping
/// </summary>
public interface IStockDataMapperService
{
    /// <summary>
    /// Maps company profile and stock price data to a StockTrade model
    /// </summary>
    StockTrade MapToStockTrade(
        string stockSymbol,
        Dictionary<string, object>? companyProfile,
        Dictionary<string, object>? stockPriceQuote,
        uint quantity = 1);
}

