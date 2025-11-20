using SmartTrade.Application.DTOs;

namespace SmartTrade.Application.Interfaces;

public interface ISellOrderService
{
  Task<SellOrderResponse> CreateSellOrder(SellOrderRequest? sellOrderRequest);
  Task<List<SellOrderResponse>> GetAllSellOrders();
}

