using System;
using SmartTrade.Application.DTOs;
using SmartTrade.Models;

namespace SmartTrade.Models.Entities;

public class Orders
{
   public List<BuyOrderResponse> BuyOrders { get; set; }
   public List<SellOrderResponse> SellOrders { get; set; }
}
