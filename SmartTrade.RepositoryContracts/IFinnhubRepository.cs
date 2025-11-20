using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmartTrade.RepositoryContracts;

/// <summary>
/// Repository interface for Finnhub API data access operations
/// </summary>
public interface IFinnhubRepository
{
    /// <summary>
    /// Gets company profile for a stock symbol
    /// </summary>
    Task<Dictionary<string, object>?> GetCompanyProfile(string stockSymbol);
    
    /// <summary>
    /// Gets stock price quote for a stock symbol
    /// </summary>
    Task<Dictionary<string, object>?> GetStockPriceQuote(string stockSymbol);
    
    /// <summary>
    /// Gets list of all stocks from US exchanges
    /// </summary>
    Task<List<Dictionary<string, string>>?> GetStocks();
    
    /// <summary>
    /// Searches for stocks by symbol or company name
    /// </summary>
    Task<List<Dictionary<string, string>>?> SearchStocks(string query);
}

