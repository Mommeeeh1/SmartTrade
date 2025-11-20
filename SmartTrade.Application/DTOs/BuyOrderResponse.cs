using System;

namespace SmartTrade.Application.DTOs;

public class BuyOrderResponse : IOrderResponse
{
    public Guid BuyOrderID { get; set; }
    public Guid OrderID => BuyOrderID;
    public string? StockSymbol { get; set; }
    public string? StockName { get; set; }
    public DateTime DateAndTimeOfOrder { get; set; }
    public uint Quantity { get; set; }
    public double Price { get; set; }
    public double TradeAmount { get; set; }
    public OrderType TypeOfOrder => OrderType.BuyOrder;
}

