using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SmartTrade.Core.Domain.Entities;

namespace SmartTrade.Infrastructure.Data;

/// <summary>
/// DbContext that inherits from IdentityDbContext to support ASP.NET Core Identity.
/// This provides Identity tables (Users, Roles, UserRoles, etc.) in addition to our custom tables.
/// </summary>
public class StockMarketDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
{
    // Constructor
    public StockMarketDbContext(DbContextOptions<StockMarketDbContext> options)
        : base(options)
    {
    }

    // Custom Tables
    public DbSet<BuyOrder> BuyOrders { get; set; }
    public DbSet<SellOrder> SellOrders { get; set; }
    
    // Identity tables (Users, Roles, UserRoles, etc.) are automatically provided by IdentityDbContext
}

