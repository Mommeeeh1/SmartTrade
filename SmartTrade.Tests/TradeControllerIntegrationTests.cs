using System;
using System.Net;
using FluentAssertions;

namespace SmartTrade.Tests;

public class TradeControllerIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public TradeControllerIntegrationTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Index_ReturnsViewWithStockTrade()
    {
        // Arrange - Nothing neeeded, client is already ready

        // Act

        HttpResponseMessage response = await _client.GetAsync("/");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        string content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("class=\"price\"");


    }
}
