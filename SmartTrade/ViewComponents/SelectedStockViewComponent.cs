using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SmartTrade.Application.Interfaces;

namespace SmartTrade.ViewComponents;

public class SelectedStockViewComponent : ViewComponent
{
    private readonly IFinnhubService _finnhubService;

    public SelectedStockViewComponent(IFinnhubService finnhubService)
    {
        _finnhubService = finnhubService;
    }

    public async Task<IViewComponentResult> InvokeAsync(string stockSymbol)
    {
        if(string.IsNullOrEmpty(stockSymbol))
        {
            return View(new Dictionary<string, object>());
        }

        Dictionary<string, object>? companyProfile = await _finnhubService.GetCompanyProfile(stockSymbol);

        Dictionary<string, object>? stockPriceQuote = await _finnhubService.GetStockPriceQuote(stockSymbol);

        Dictionary<string, object> stockDetails = new Dictionary<string, object>();

        if(companyProfile != null)
        {
            foreach (var item in companyProfile)
            {
                stockDetails[item.Key] = item.Value;
            }
        }

        if(stockPriceQuote != null)
        {
            // Add price from stock quote (key "c" is the current price)
            if (stockPriceQuote.ContainsKey("c"))
            {
                stockDetails["price"] = stockPriceQuote["c"];
            }
        }

        if (!stockDetails.ContainsKey("ticker"))
        {
            stockDetails["ticker"] = stockSymbol;
        }

        return View(stockDetails);

    } 
}
