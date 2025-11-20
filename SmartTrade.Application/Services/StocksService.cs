using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SmartTrade.Application.Interfaces;
using SmartTrade.Application.DTOs;
using SmartTrade.Core.Domain.Entities;
using SmartTrade.Application.Helpers;
using SmartTrade.Core.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace SmartTrade.Application.Services;

public class StocksService : IBuyOrderService, ISellOrderService
{
    private readonly IStocksRepository _repository;
    private readonly ILogger<StocksService> _logger;
    public StocksService(IStocksRepository repository, ILogger<StocksService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public  async Task<BuyOrderResponse> CreateBuyOrder(BuyOrderRequest? buyOrderRequest)
    {  
        _logger.LogInformation("CreateBuyOrder method called for stock: {StockSymbol}", buyOrderRequest?.StockSymbol ?? "null");
        
        // Null check
        if (buyOrderRequest == null)
        {
            _logger.LogWarning("Buy order request is null");
            throw new ArgumentNullException(nameof(buyOrderRequest));
        }

        _logger.LogInformation("Creating buy order for {StockSymbol}, Quantity: {Quantity}, Price: {Price}", 
            buyOrderRequest.StockSymbol, 
            buyOrderRequest.Quantity,
            buyOrderRequest.Price);

        // Model validation using Data Annotations
        ValidationHelper.ModelValidation(buyOrderRequest);

        // Custom date validation (not covered by Data Annotations)
        if (buyOrderRequest.DateAndTimeOfOrder < new DateTime(2000, 1, 1))
        {
            _logger.LogWarning("Invalid date provided: {Date}", buyOrderRequest.DateAndTimeOfOrder);
            throw new ArgumentException("Date must be on or after January 1, 2000", nameof(buyOrderRequest.DateAndTimeOfOrder));
        }

        BuyOrder buyOrder = new BuyOrder
        {
            BuyOrderID = Guid.NewGuid(),
            StockSymbol = buyOrderRequest.StockSymbol,
            StockName = buyOrderRequest.StockName,
            DateAndTimeOfOrder = buyOrderRequest.DateAndTimeOfOrder,
            Quantity = buyOrderRequest.Quantity,
            Price = buyOrderRequest.Price,
        };

        await _repository.CreateBuyOrder(buyOrder);

        BuyOrderResponse response = new BuyOrderResponse
        {
            BuyOrderID = buyOrder.BuyOrderID,
            StockSymbol = buyOrder.StockSymbol,
            StockName = buyOrder.StockName,
            DateAndTimeOfOrder = buyOrder.DateAndTimeOfOrder,
            Quantity = buyOrder.Quantity,
            Price = buyOrder.Price,
            TradeAmount = buyOrder.Price * buyOrder.Quantity
        };

        _logger.LogInformation("Buy order created successfully. OrderID: {OrderID}, TradeAmount: {TradeAmount}", 
            response.BuyOrderID, 
            response.TradeAmount);

        return response;
    }

    public async Task<SellOrderResponse> CreateSellOrder(SellOrderRequest? sellOrderRequest)
    {
        _logger.LogInformation("CreateSellOrder method called for stock: {StockSymbol}", sellOrderRequest?.StockSymbol ?? "null");
        
        // Null check
        if (sellOrderRequest == null)
        {
            _logger.LogWarning("Sell order request is null");
            throw new ArgumentNullException(nameof(sellOrderRequest));
        }

        _logger.LogInformation("Creating sell order for {StockSymbol}, Quantity: {Quantity}, Price: {Price}", 
            sellOrderRequest.StockSymbol, 
            sellOrderRequest.Quantity,
            sellOrderRequest.Price);

        // Model validation using Data Annotations
        ValidationHelper.ModelValidation(sellOrderRequest);

        // Custom date validation (not covered by Data Annotations)
        if (sellOrderRequest.DateAndTimeOfOrder < new DateTime(2000, 1, 1))
        {
            _logger.LogWarning("Invalid date provided: {Date}", sellOrderRequest.DateAndTimeOfOrder);
            throw new ArgumentException("Date must be on or after January 1, 2000", nameof(sellOrderRequest.DateAndTimeOfOrder));
        }

        SellOrder sellOrder = new SellOrder
        {
            SellOrderID = Guid.NewGuid(),
            StockSymbol = sellOrderRequest.StockSymbol,
            StockName = sellOrderRequest.StockName,
            DateAndTimeOfOrder = sellOrderRequest.DateAndTimeOfOrder,
            Quantity = sellOrderRequest.Quantity,
            Price = sellOrderRequest.Price,
        };

        await _repository.CreateSellOrder(sellOrder);

        SellOrderResponse response = new SellOrderResponse
        {
            SellOrderID = sellOrder.SellOrderID,
            StockSymbol = sellOrder.StockSymbol,
            StockName = sellOrder.StockName,
            DateAndTimeOfOrder = sellOrder.DateAndTimeOfOrder,
            Quantity = sellOrder.Quantity,
            Price = sellOrder.Price,
            TradeAmount = sellOrder.Price * sellOrder.Quantity
        };

        _logger.LogInformation("Sell order created successfully. OrderID: {OrderID}, TradeAmount: {TradeAmount}", 
            response.SellOrderID, 
            response.TradeAmount);

        return response;
    }

    public async Task<List<BuyOrderResponse>> GetAllBuyOrders()
    {
        _logger.LogInformation("Fetching all buy orders");
        
        List<BuyOrderResponse> response = new List<BuyOrderResponse>();

        foreach (BuyOrder buyOrder in await _repository.GetBuyOrders())
        {
            BuyOrderResponse buyOrderResponse = new BuyOrderResponse
            {
                BuyOrderID = buyOrder.BuyOrderID,
                StockSymbol = buyOrder.StockSymbol,
                StockName = buyOrder.StockName,
                DateAndTimeOfOrder = buyOrder.DateAndTimeOfOrder,
                Quantity = buyOrder.Quantity,
                Price = buyOrder.Price,
                TradeAmount = buyOrder.Price * buyOrder.Quantity
            };

            response.Add(buyOrderResponse);
        }

        _logger.LogInformation("Retrieved {Count} buy orders", response.Count);
        return response;
    }

    public async Task<List<SellOrderResponse>> GetAllSellOrders()
    {
        List<SellOrderResponse> response = new List<SellOrderResponse>();

        foreach (SellOrder sellOrder in await _repository.GetSellOrders())
        {
            SellOrderResponse sellOrderResponse = new SellOrderResponse
            {
                SellOrderID = sellOrder.SellOrderID,
                StockSymbol = sellOrder.StockSymbol,
                StockName = sellOrder.StockName,
                DateAndTimeOfOrder = sellOrder.DateAndTimeOfOrder,
                Quantity = sellOrder.Quantity,
                Price = sellOrder.Price,
                TradeAmount = sellOrder.Price * sellOrder.Quantity
            };

            response.Add(sellOrderResponse);
        }

        return response;
    }
}

