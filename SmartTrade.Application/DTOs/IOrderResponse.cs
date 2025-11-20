using System;

namespace SmartTrade.Application.DTOs;

public interface IOrderResponse
{
    Guid OrderID { get; }
    string? StockSymbol { get; }
    string? StockName { get; }
    DateTime DateAndTimeOfOrder { get; }
    uint Quantity { get; }
    double Price { get; }
    double TradeAmount { get; }
    OrderType TypeOfOrder { get; }
}

public enum OrderType
{
    BuyOrder,
    SellOrder
}

