using Microsoft.EntityFrameworkCore;

namespace SmartTrade.Entities;

public class StockMarketDbContext : DbContext
{
    // Constructor
    public StockMarketDbContext(DbContextOptions<StockMarketDbContext> options)
        : base(options)
    {
    }

    // Tables
    public DbSet<BuyOrder> BuyOrders { get; set; }
    public DbSet<SellOrder> SellOrders { get; set; }
}

