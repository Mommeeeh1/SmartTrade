using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using SmartTrade.Application.Interfaces;

namespace SmartTrade.Tests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        
        builder.ConfigureServices(services =>
        {
            // Remove the real IFinnhubService
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(IFinnhubService));
            if (descriptor != null)
            {
                services.Remove(descriptor);
            }
            
            // Add a mock IFinnhubService
            var mockFinnhubService = new Mock<IFinnhubService>();
            
            // Setup mock to return fake company profile
            mockFinnhubService
                .Setup(x => x.GetCompanyProfile(It.IsAny<string>()))
                .ReturnsAsync(new Dictionary<string, object>
                {
                    { "name", "Microsoft Corporation" },
                    { "ticker", "MSFT" }
                });
            
            // Setup mock to return fake stock price
            mockFinnhubService
                .Setup(x => x.GetStockPriceQuote(It.IsAny<string>()))
                .ReturnsAsync(new Dictionary<string, object>
                {
                    { "c", 350.50 }  // Current price
                });
            
            services.AddSingleton(mockFinnhubService.Object);
        });
    }
}