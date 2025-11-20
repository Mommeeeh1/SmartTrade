using System.Threading.Tasks;
using SmartTrade.Application.DTOs;

namespace SmartTrade.Application.Interfaces;

/// <summary>
/// Service for managing buy orders
/// Follows Interface Segregation Principle: Only contains buy order operations
/// </summary>
public interface IBuyOrderService
{
    Task<BuyOrderResponse> CreateBuyOrder(BuyOrderRequest? buyOrderRequest);
    Task<List<BuyOrderResponse>> GetAllBuyOrders();
}

