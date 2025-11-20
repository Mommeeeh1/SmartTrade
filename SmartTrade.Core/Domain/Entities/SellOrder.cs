using System;
using System.ComponentModel.DataAnnotations;

namespace SmartTrade.Core.Domain.Entities;

public class SellOrder
{
    public Guid SellOrderID { get; set; }

    [Required(ErrorMessage = "Stock Symbol is required")]
    public string? StockSymbol { get; set; }

    [Required(ErrorMessage = "Stock Name is required")]
    public string? StockName { get; set; }

    public DateTime DateAndTimeOfOrder { get; set; }

    [Range(1, 100000, ErrorMessage = "Quantity must be between 1 and 100,000")]
    public uint Quantity { get; set; }

    [Range(1, 10000, ErrorMessage = "Price must be between 1 and 10,000")]
    public double Price { get; set; }
}

