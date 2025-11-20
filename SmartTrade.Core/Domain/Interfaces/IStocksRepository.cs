using System.Collections.Generic;
using System.Threading.Tasks;
using SmartTrade.Core.Domain.Entities;

namespace SmartTrade.Core.Domain.Interfaces;

/// <summary>
/// Repository interface for Stocks data access operations
/// </summary>
public interface IStocksRepository
{
    /// <summary>
    /// Inserts a new buy order into the database
    /// </summary>
    Task<BuyOrder> CreateBuyOrder(BuyOrder buyOrder);
    
    /// <summary>
    /// Inserts a new sell order into the database
    /// </summary>
    Task<SellOrder> CreateSellOrder(SellOrder sellOrder);
    
    /// <summary>
    /// Retrieves all buy orders from the database
    /// </summary>
    Task<List<BuyOrder>> GetBuyOrders();
    
    /// <summary>
    /// Retrieves all sell orders from the database
    /// </summary>
    Task<List<SellOrder>> GetSellOrders();
}

