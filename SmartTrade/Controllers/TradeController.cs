using System;
using Microsoft.AspNetCore.Mvc;
using SmartTrade.Application.Interfaces;
using SmartTrade.Application.DTOs;
using SmartTrade.Core.Domain.Entities;
using SmartTrade.Infrastructure.Configuration;
using System.Globalization;
using Microsoft.Extensions.Options;
using SmartTrade.Models.Entities;
using Rotativa.AspNetCore;
using SmartTrade.Infrastructure.Filters;

namespace SmartTrade.Controllers;

// TradeController requires authentication (FallbackPolicy will enforce this)
// Users must login to access trading pages
public class TradeController : Controller
{
    private readonly IFinnhubService _finnhubService;
    private readonly IOptions<TradingOptions> _tradingOptions;
    private readonly IBuyOrderService _buyOrderService;
    private readonly ISellOrderService _sellOrderService;      
    private readonly IStockDataMapperService _stockDataMapper;
    private readonly ILogger<TradeController> _logger;
    public TradeController(IFinnhubService finnhubService, IOptions<TradingOptions> tradingOptions, IBuyOrderService buyOrderService, ISellOrderService sellOrderService, IStockDataMapperService stockDataMapper, ILogger<TradeController> logger)
    {
        _finnhubService = finnhubService;
        _tradingOptions = tradingOptions;
        _buyOrderService = buyOrderService;
        _sellOrderService = sellOrderService;
        _stockDataMapper = stockDataMapper;
        _logger = logger;
    }


   [Route("/")]
    public async Task<IActionResult> Index()
    {
        // TEMPORARY: Test exception handling - REMOVE AFTER TESTING!
        //throw new Exception("Test exception - This should be caught by ExceptionHandlingMiddleware!");
        
        try
        {
            _logger.LogInformation("Index page requested for stock: {StockSymbol}", _tradingOptions.Value.DefaultStockSymbol);
            
            var companyProfile = await _finnhubService.GetCompanyProfile(_tradingOptions.Value.DefaultStockSymbol);
            _logger.LogInformation("Company profile retrieved for {StockSymbol}", _tradingOptions.Value.DefaultStockSymbol);
            
            var stockPriceQuote = await _finnhubService.GetStockPriceQuote(_tradingOptions.Value.DefaultStockSymbol);
            
            var stockTrade = _stockDataMapper.MapToStockTrade(
                _tradingOptions.Value.DefaultStockSymbol,
                companyProfile,
                stockPriceQuote,
                quantity: 1
            );


            _logger.LogInformation("Stock trade data prepared for {StockSymbol}, Price: {Price}", 
                stockTrade.StockSymbol, 
                stockTrade.Price);

            ViewBag.FinnhubToken = _tradingOptions.Value.FinnhubToken;
            return View(stockTrade);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading Index page for stock: {StockSymbol}", _tradingOptions.Value.DefaultStockSymbol);
            throw;
        }
    }

    [HttpPost]
[Route("/Trade/BuyOrder")]
[ServiceFilter(typeof(CreateOrderActionFilter))]  // ✅ ADD THIS LINE
public async Task<IActionResult> BuyOrder(BuyOrderRequest buyOrderRequest)
    {
        _logger.LogInformation("BuyOrder action called for stock: {StockSymbol}", buyOrderRequest?.StockSymbol);
        
        // Set the trade date and time to the current date and time
        buyOrderRequest.DateAndTimeOfOrder = DateTime.Now;

        // ❌ REMOVE ALL THIS VALIDATION CODE (filter handles it now!)
        // The filter already checked ModelState, so we can skip it here

        // Call the service to create the new buy order
        BuyOrderResponse response = await _buyOrderService.CreateBuyOrder(buyOrderRequest);
        
        _logger.LogInformation("Buy order successful, redirecting to Orders page");

        // Redirect to the Orders page
        return RedirectToAction("Orders");
    }

    [HttpPost]
    [Route("/Trade/SellOrder")]
    [ServiceFilter(typeof(CreateOrderActionFilter))]  // ✅ ADD THIS LINE
    public async Task<IActionResult> SellOrder(SellOrderRequest sellOrderRequest)
    {
        // Set the trade date and time to the current date and time
        sellOrderRequest.DateAndTimeOfOrder = DateTime.Now;
        
        
        // Call the service to create the new sell order
        SellOrderResponse response = await _sellOrderService.CreateSellOrder(sellOrderRequest);
        
        // Redirect to the Orders page
        return RedirectToAction("Orders");
    }

    [Route("/Trade/Orders")]
    public async Task<IActionResult> Orders()
    {
        _logger.LogInformation("Orders page requested");
        
        // Get all buy orders
        List<BuyOrderResponse> buyOrders = await _buyOrderService.GetAllBuyOrders();
        // Get all sell orders
        List<SellOrderResponse> sellOrders = await _sellOrderService.GetAllSellOrders();

        _logger.LogInformation("Retrieved {BuyOrderCount} buy orders and {SellOrderCount} sell orders", 
            buyOrders.Count, 
            sellOrders.Count);

        // Create a new Orders object
        Orders orders = new Orders
        {
            BuyOrders = buyOrders,
            SellOrders = sellOrders
        };

        // Return the view with the orders
        return View(orders);
        
    }

    [Route("/Trade/OrdersPDF")]
    public async Task<IActionResult> OrdersPDF()
    {
        // Get all buy orders
        List<BuyOrderResponse> buyOrders = await _buyOrderService.GetAllBuyOrders();
        // Get all sell orders
        List<SellOrderResponse> sellOrders = await _sellOrderService.GetAllSellOrders();

        // Combine both into a single list of IOrderResponse
        List<IOrderResponse> allOrders = [..buyOrders, ..sellOrders];

        // Sort date (newest to oldest)
        allOrders = allOrders.OrderByDescending(order => order.DateAndTimeOfOrder).ToList();

       return new ViewAsPdf("OrdersPDF", allOrders)
    {
        FileName = $"Orders_{DateTime.Now:yyyyMMdd_HHmmss}.pdf"
    };
    }


    
}
