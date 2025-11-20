using Microsoft.AspNetCore.Identity;

namespace SmartTrade.Core.Domain.Entities;

/// <summary>
/// ApplicationUser class that extends IdentityUser with Guid as the primary key type.
/// This class represents a user in the application and can be extended with additional properties.
/// </summary>
public class ApplicationUser : IdentityUser<Guid>
{
    // You can add additional properties here as needed
    // Example:
    // public string? FirstName { get; set; }
    // public string? LastName { get; set; }
    // public DateTime DateOfBirth { get; set; }
}

