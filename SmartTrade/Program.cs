using Serilog;
using Serilog.Events;
using Rotativa.AspNetCore;
using SmartTrade.Infrastructure.Extensions;
using SmartTrade.Infrastructure.Middleware;
using SmartTrade.Infrastructure.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using SmartTrade.Core.Domain.Entities;
using SmartTrade.Infrastructure.Data;

// Configure Serilog
var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)  // Reduce noise from Microsoft logs
    .MinimumLevel.Is(environment == "Development" ? LogEventLevel.Debug : LogEventLevel.Information)  // Debug in Dev, Info in Prod
    .WriteTo.Console()
    .WriteTo.File("logs/smarttrade-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

// Use Serilog for logging
builder.Host.UseSerilog();

// MVC Services
builder.Services.AddControllersWithViews();

// Register all application services
builder.Services.AddApplicationServices(builder.Configuration);

// ASP.NET Core Identity Configuration
// (Must be in Web project as it requires web framework references)
builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
{
    // Password Complexity Configuration
    options.Password.RequiredLength = 6; // Minimum number of characters required in password
    options.Password.RequireNonAlphanumeric = true; // Require at least one non-alphanumeric character (symbols)
    options.Password.RequireUppercase = true; // Require at least one uppercase character
    options.Password.RequireLowercase = true; // Require at least one lowercase character
    options.Password.RequireDigit = true; // Require at least one digit (0-9)
    options.Password.RequiredUniqueChars = 1; // Number of distinct characters required in password
    
    // User Options (optional - you can customize these)
    options.User.RequireUniqueEmail = true; // Each user must have a unique email
    
    // Configure login path - redirect unauthenticated users to login page
    options.SignIn.RequireConfirmedEmail = false; // Set to true if you want email confirmation
})
.AddEntityFrameworkStores<StockMarketDbContext>()
.AddDefaultTokenProviders();

// Configure cookie authentication options
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login"; // Redirect to login page when authentication is required
    options.LogoutPath = "/Account/Logout";
    options.AccessDeniedPath = "/Account/Login"; // Redirect here if user tries to access unauthorized resource
    options.ExpireTimeSpan = TimeSpan.FromMinutes(30); // Cookie expires after 30 minutes of inactivity
    options.SlidingExpiration = true; // Reset expiration time on each request
});

// Authorization Policy Configuration
// FallbackPolicy requires all users to be authenticated by default
// You can override this on specific controllers/actions using [AllowAnonymous]
builder.Services.AddAuthorization(options =>
{
    var policy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
    options.FallbackPolicy = policy;
});

// Register filters (web-specific, so registered here)
builder.Services.AddScoped<CreateOrderActionFilter>();

var app = builder.Build();

// Configure Rotativa (for PDF generation)
Rotativa.AspNetCore.RotativaConfiguration.Setup(app.Environment.WebRootPath, "Rotativa");

// Exception handling - must be early in pipeline
if (!app.Environment.IsDevelopment())
{
    // Production: Built-in error handler - shows friendly error page
    app.UseExceptionHandler("/Home/Error");
}
// Development: Let exceptions bubble up for detailed error pages

// Custom exception logging middleware - logs errors then rethrows
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseStaticFiles();
app.UseRouting();

// Authentication & Authorization Middleware
// IMPORTANT: Order matters! Authentication must come before Authorization
app.UseAuthentication(); // Must come after UseRouting()
app.UseAuthorization();  // Must come after UseAuthentication()

// MVC Routing Configuration
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Trade}/{action=Index}/{id?}");

app.Run();

public partial class Program { }


