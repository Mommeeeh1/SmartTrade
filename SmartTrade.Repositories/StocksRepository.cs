using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SmartTrade.Entities;
using SmartTrade.RepositoryContracts;

namespace SmartTrade.Repositories;

/// <summary>
/// Repository implementation for Stocks data access operations
/// </summary>
public class StocksRepository : IStocksRepository
{
    private readonly StockMarketDbContext _dbContext;

    /// <summary>
    /// Constructor - receives DbContext via Dependency Injection
    /// </summary>
    public StocksRepository(StockMarketDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<BuyOrder> CreateBuyOrder(BuyOrder buyOrder)
    {   
        // Add the buy order to the DbSet
        _dbContext.BuyOrders.Add(buyOrder);

        // Save changes to the database
        await _dbContext.SaveChangesAsync();

        // Return the saved buy order
        return buyOrder;
    }

    public async Task<SellOrder> CreateSellOrder(SellOrder sellOrder)
    {
        // Add the sell order to the DbSet
        _dbContext.SellOrders.Add(sellOrder);

        // Save changes to the database
        await _dbContext.SaveChangesAsync();

        // Return the saved sell order
        return sellOrder;
    }

    public async Task<List<BuyOrder>> GetBuyOrders()
    {
        // Retrieve all buy orders from the database
        return await _dbContext.BuyOrders.ToListAsync();
    }

    public async Task<List<SellOrder>> GetSellOrders()
    {
        // Retrieve all sell orders from the database
        return await _dbContext.SellOrders.ToListAsync();
    }
}

