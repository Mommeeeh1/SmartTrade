using System;

namespace SmartTrade.Application.DTOs;

public class SellOrderResponse : IOrderResponse
{
    public Guid SellOrderID { get; set; }
    public Guid OrderID => SellOrderID;
    public string? StockSymbol { get; set; }
    public string? StockName { get; set; }
    public DateTime DateAndTimeOfOrder { get; set; }
    public uint Quantity { get; set; }
    public double Price { get; set; }
    public double TradeAmount { get; set; }
    public OrderType TypeOfOrder => OrderType.SellOrder;
}

