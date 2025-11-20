using System;
using SmartTrade.ServiceContracts.DTO;

namespace SmartTrade.ServiceContracts;

public interface ISellOrderService
{
  Task<SellOrderResponse> CreateSellOrder(SellOrderRequest? sellOrderRequest);
  Task<List<SellOrderResponse>> GetAllSellOrders();
}
