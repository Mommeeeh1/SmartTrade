using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SmartTrade.Infrastructure.Configuration;
using SmartTrade.Application.Interfaces;
using SmartTrade.Application.DTOs;
using SmartTrade.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartTrade.Controllers
{
    // StocksController requires authentication (FallbackPolicy will enforce this)
    public class StocksController : Controller
    {

         private readonly IOptions<TradingOptions> _tradingOptions;
         private readonly IFinnhubService _finnhubService;

         public StocksController(IOptions<TradingOptions> tradingOptions, IFinnhubService finnhubService)
         {
            _tradingOptions = tradingOptions;
            _finnhubService = finnhubService;
         }
        
[HttpGet]
[Route("/Stocks/Explore/{stock?}")]
public async Task<IActionResult> Explore(string? stock)
{
    // Get all stocks from Finnhub
    List<Dictionary<string, string>>? stocksFromFinnhub = await _finnhubService.GetStocks();
    
    // Get the Top25PopularStocks from configuration (comma-separated)
    string? top25StocksString = _tradingOptions.Value.Top25PopularStocks;
    
    // Split the comma-separated string into a List
    List<string>? top25StocksList = top25StocksString?.Split(',').ToList();
    
    // Filter stocks - keep only those in Top 25
    List<Dictionary<string, string>>? filteredStocks = null;
    
    if (stocksFromFinnhub != null && top25StocksList != null)
    {
        filteredStocks = stocksFromFinnhub
            .Where(s => top25StocksList.Contains(Convert.ToString(s["symbol"])))
            .ToList();
    }
    
    // Convert dictionaries to Stock objects
    List<Stock> stocks = new List<Stock>();
    if (filteredStocks != null)
    {
        stocks = filteredStocks
            .Select(s => new Stock
            {
                StockSymbol = s["symbol"]?.ToString() ?? "",
                StockName = s["description"]?.ToString() ?? ""
            })
            .ToList();
    }

    // Pass the selected stock symbol to view via ViewBag
    ViewBag.SelectedStock = stock;

    // Return the view with Stock Objects
    return View(stocks);
    }
}
}
