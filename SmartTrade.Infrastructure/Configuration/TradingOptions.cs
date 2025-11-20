using System;

namespace SmartTrade.Infrastructure.Configuration;

public class TradingOptions
{
    public string? DefaultStockSymbol { get; set; }
    public string? FinnhubToken { get; set; }
    public string? Top25PopularStocks { get; set; }  
}

