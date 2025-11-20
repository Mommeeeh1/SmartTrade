using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Http;
using SmartTrade.Core.Domain.Interfaces;

namespace SmartTrade.Infrastructure.ExternalServices;

/// <summary>
/// Repository implementation for Finnhub API data access operations
/// </summary>
public class FinnhubRepository : IFinnhubRepository
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;

    /// <summary>
    /// Constructor - receives dependencies via Dependency Injection
    /// </summary>
    public FinnhubRepository(IConfiguration configuration, IHttpClientFactory httpClientFactory)
    {
        _configuration = configuration;
        _httpClientFactory = httpClientFactory;
    }
     
    public async Task<Dictionary<string, object>?> GetCompanyProfile(string stockSymbol)
    {
        try
        {
            HttpClient httpClient = _httpClientFactory.CreateClient();
            string token = _configuration.GetValue<string>("TradingOptions:FinnhubToken") ?? "";
            string url = $"https://finnhub.io/api/v1/stock/profile2?symbol={stockSymbol}&token={token}";
            HttpResponseMessage response = await httpClient.GetAsync(url);
            string responseBody = await response.Content.ReadAsStringAsync();
            
            if (!response.IsSuccessStatusCode)
            {
                // API error - return empty dictionary
                return new Dictionary<string, object>();
            }
            
            return JsonSerializer.Deserialize<Dictionary<string, object>>(responseBody);
        }
        catch
        {
            // If API fails, return empty dictionary instead of crashing
            return new Dictionary<string, object>();
        }
    }

    public async Task<Dictionary<string, object>?> GetStockPriceQuote(string stockSymbol)
    {
        try
        {
            HttpClient httpClient = _httpClientFactory.CreateClient();
            string token = _configuration.GetValue<string>("TradingOptions:FinnhubToken") ?? "";
            string url = $"https://finnhub.io/api/v1/quote?symbol={stockSymbol}&token={token}";
            HttpResponseMessage response = await httpClient.GetAsync(url);
            string responseBody = await response.Content.ReadAsStringAsync();
            
            if (!response.IsSuccessStatusCode)
            {
                // API error - return dictionary with default price
                return new Dictionary<string, object> { { "c", 100.0 } };
            }
            
            return JsonSerializer.Deserialize<Dictionary<string, object>>(responseBody);
        }
        catch
        {
            // If API fails, return dictionary with default price
            return new Dictionary<string, object> { { "c", 100.0 } };
        }
    }

    public async Task<List<Dictionary<string, string>>?> GetStocks()
    {
        HttpClient httpClient = _httpClientFactory.CreateClient();
        string token = _configuration.GetValue<string>("TradingOptions:FinnhubToken") ?? "";
        string url = $"https://finnhub.io/api/v1/stock/symbol?exchange=US&token={token}";
        HttpResponseMessage response = await httpClient.GetAsync(url);
        string responseBody = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<List<Dictionary<string, string>>>(responseBody);
    }

    public async Task<List<Dictionary<string, string>>?> SearchStocks(string query)
    {
        HttpClient httpClient = _httpClientFactory.CreateClient();
        string token = _configuration.GetValue<string>("TradingOptions:FinnhubToken") ?? "";
        string url = $"https://finnhub.io/api/v1/search?q={query}&token={token}";
        HttpResponseMessage response = await httpClient.GetAsync(url);
        string responseBody = await response.Content.ReadAsStringAsync();
        
        // Finnhub search API returns an object with a "result" array
        var searchResult = JsonSerializer.Deserialize<Dictionary<string, object>>(responseBody);
        
        if (searchResult != null && searchResult.ContainsKey("result"))
        {
            var resultJson = searchResult["result"]?.ToString();
            if (!string.IsNullOrEmpty(resultJson))
            {
                return JsonSerializer.Deserialize<List<Dictionary<string, string>>>(resultJson);
            }
        }
        
        return new List<Dictionary<string, string>>();
    }
}

