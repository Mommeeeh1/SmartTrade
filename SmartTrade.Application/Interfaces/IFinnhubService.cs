using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmartTrade.Application.Interfaces;

public interface IFinnhubService
{
    Task<Dictionary<string, object>?> GetCompanyProfile(string stockSymbol);
    Task<Dictionary<string, object>?> GetStockPriceQuote(string stockSymbol);
    Task<List<Dictionary<string, string>>?> GetStocks();
    Task<List<Dictionary<string, string>>?> SearchStocks(string query);
}

