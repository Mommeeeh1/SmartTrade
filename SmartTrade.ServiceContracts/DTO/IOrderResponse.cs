using System;

namespace SmartTrade.ServiceContracts.DTO;

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