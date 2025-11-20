using System.Collections.Generic;
using System.Threading.Tasks;
using SmartTrade.ServiceContracts;
using SmartTrade.RepositoryContracts;

namespace SmartTrade.Services;

public class FinnhubService : IFinnhubService
{
   private readonly IFinnhubRepository _repository;

    public FinnhubService(IFinnhubRepository repository)
    {
        _repository = repository;
    }

    public async Task<Dictionary<string, object>?> GetCompanyProfile(string stockSymbol)
    {
        return await _repository.GetCompanyProfile(stockSymbol);
    }

    public async Task<Dictionary<string, object>?> GetStockPriceQuote(string stockSymbol)
    {
        return await _repository.GetStockPriceQuote(stockSymbol);
    }

    public async Task<List<Dictionary<string, string>>?> GetStocks()
    {
        return await _repository.GetStocks();
    }

    public async Task<List<Dictionary<string, string>>?> SearchStocks(string query)
    {
        return await _repository.SearchStocks(query);
    }
}

