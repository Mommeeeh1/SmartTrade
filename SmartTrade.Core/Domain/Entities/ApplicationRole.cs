using Microsoft.AspNetCore.Identity;

namespace SmartTrade.Core.Domain.Entities;

/// <summary>
/// ApplicationRole class that extends IdentityRole with Guid as the primary key type.
/// This class represents a role in the application (e.g., "Admin", "User", "Customer").
/// You can add additional properties to this class as needed.
/// </summary>
public class ApplicationRole : IdentityRole<Guid>
{
    // You can add additional properties here as needed
    // Example:
    // public string? Description { get; set; }
}

