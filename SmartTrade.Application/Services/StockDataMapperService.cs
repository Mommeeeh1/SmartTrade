using System.Globalization;
using SmartTrade.Core.Domain.Entities;
using SmartTrade.Application.Interfaces;

namespace SmartTrade.Application.Services;

/// <summary>
/// Implementation of IStockDataMapperService
/// Handles mapping of raw API data to application models
/// Follows Single Responsibility Principle
/// </summary>
public class StockDataMapperService : IStockDataMapperService
{
    /// <summary>
    /// Maps company profile and stock price data to a StockTrade model
    /// </summary>
    public StockTrade MapToStockTrade(
        string stockSymbol,
        Dictionary<string, object>? companyProfile,
        Dictionary<string, object>? stockPriceQuote,
        uint quantity = 1)
    {
        // Extract company name from profile
        string stockName = ExtractStockName(companyProfile);
        
        // Extract price from quote
        double price = ExtractPrice(stockPriceQuote);
        
        // Create and return the StockTrade model
        return new StockTrade
        {
            StockSymbol = stockSymbol,
            StockName = stockName,
            Price = price,
            Quantity = quantity
        };
    }
    
    /// <summary>
    /// Extracts stock name from company profile dictionary
    /// Returns "Unknown" if data is missing or invalid
    /// </summary>
    private string ExtractStockName(Dictionary<string, object>? companyProfile)
    {
        // Check if profile exists and contains "name" key
        if (companyProfile != null && companyProfile.ContainsKey("name"))
        {
            // Try to get name value, fallback to "Unknown"
            return companyProfile["name"]?.ToString() ?? "Unknown";
        }
        
        return "Unknown";
    }
    
    /// <summary>
    /// Extracts price from stock price quote dictionary
    /// Returns 0 if data is missing or invalid
    /// </summary>
    private double ExtractPrice(Dictionary<string, object>? stockPriceQuote)
    {
        // Check if quote exists and contains "c" (current price) key
        if (stockPriceQuote != null && stockPriceQuote.ContainsKey("c"))
        {
            string? priceString = stockPriceQuote["c"]?.ToString();
            
            if (!string.IsNullOrEmpty(priceString))
            {
                // Try to parse as double with InvariantCulture
                if (double.TryParse(priceString, NumberStyles.Any, CultureInfo.InvariantCulture, out double price))
                {
                    return price;
                }
            }
        }
        
        return 0;
    }
}

