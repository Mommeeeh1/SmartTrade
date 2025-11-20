using System;
using SmartTrade.Application.Services;
using SmartTrade.Application.Interfaces;
using SmartTrade.Application.DTOs;
using Xunit;
using Moq; 
using AutoFixture;
using FluentAssertions;
using SmartTrade.Core.Domain.Interfaces;
using SmartTrade.Core.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace SmartTrade.Tests;




public class StockServiceTest
{

private readonly Mock<IStocksRepository> _mockRepository;
private readonly Mock<ILogger<StocksService>> _mockLogger;
private readonly StocksService _stocksService;
private readonly IFixture _fixture;

public StockServiceTest()
{
    _mockRepository = new Mock<IStocksRepository>();
    _mockLogger = new Mock<ILogger<StocksService>>();
    _stocksService = new StocksService(_mockRepository.Object, _mockLogger.Object);
    _fixture = new Fixture();
}

    #region CreateBuyOrder Tests

    [Fact]
    public async Task CreateBuyOrder_NullRequest_ThrowsArgumentNullException()
    {
        // Arrange
        BuyOrderRequest? request = null;

        // Act

        Func<Task> act = async () => await _stocksService.CreateBuyOrder(request);

        // Assert
        await act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task CreateBuyOrder_InvalidQuantity_ThrowsArgumentException()
    {
        // Arrange
        BuyOrderRequest request = _fixture.Build<BuyOrderRequest>()
            .With(x => x.Quantity, (uint)0)
            .Create();

        // Act
        Func<Task> act = async () => await _stocksService.CreateBuyOrder(request);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>();
    }


    [Fact]

  public async Task CreateBuyOrder_QuantityExceedsMaximum_ThrowsArgumentException()
{
    // Arrange
    BuyOrderRequest request = _fixture.Build<BuyOrderRequest>()
        .With(x => x.Quantity, (uint)100001)
        .Create();
    Func<Task> act = async () => await _stocksService.CreateBuyOrder(request!);
    
    // Act & Assert
    await act.Should().ThrowAsync<ArgumentException>();
}

[Fact]
public async Task CreateBuyOrder_PriceIsZero_ThrowsArgumentException()
{
    // Arrange
    BuyOrderRequest request = _fixture.Build<BuyOrderRequest>()
        .With(x => x.Quantity, (uint)100)
        .With(x => x.Price, 0)
        .Create();
    Func<Task> act = async () => await _stocksService.CreateBuyOrder(request);
    
    // Assert
    await act.Should().ThrowAsync<ArgumentException>();
}

[Fact]
public async Task CreateBuyOrder_PriceExceedsMaximum_ThrowsArgumentException()
{
    // Arrange
    BuyOrderRequest request = _fixture.Build<BuyOrderRequest>()
        .With(x => x.Quantity, (uint)100)
        .With(x => x.Price, 10001)
        .Create();
    Func<Task> act = async () => await _stocksService.CreateBuyOrder(request);
    
    // Assert
    await act.Should().ThrowAsync<ArgumentException>();
}

[Fact]
public async Task CreateBuyOrder_StockSymbolIsNull_ThrowsArgumentException()
{
    // Arrange
    BuyOrderRequest request = _fixture.Build<BuyOrderRequest>()
        .With(x => x.StockSymbol, (string?)null)
        .With(x => x.Quantity, (uint)100)
        .With(x => x.Price, 150.00)
        .Create();
    Func<Task> act = async () => await _stocksService.CreateBuyOrder(request);
    
    // Assert
    await act.Should().ThrowAsync<ArgumentException>();
}

[Fact]
public async Task CreateBuyOrder_DateBeforeYear2000_ThrowsArgumentException()
{
    // Arrange
    BuyOrderRequest request = _fixture.Build<BuyOrderRequest>()
        .With(x => x.DateAndTimeOfOrder, new DateTime(1999, 12, 31))
        .With(x => x.Quantity, (uint)100)
        .With(x => x.Price, 150.00)
        .Create();
    Func<Task> act = async () => await _stocksService.CreateBuyOrder(request);
    
    // Assert
    await act.Should().ThrowAsync<ArgumentException>();
}


[Fact]
public async Task CreateBuyOrder_ValidRequest_ReturnsBuyOrderResponse()
{
    // Arrange
    BuyOrderRequest request = _fixture.Build<BuyOrderRequest>()
        .With(x => x.StockSymbol, "AAPL")
        .With(x => x.StockName, "Apple Inc.")
        .With(x => x.DateAndTimeOfOrder, DateTime.Now)
        .With(x => x.Quantity, (uint)100)
        .With(x => x.Price, 150.00)
        .Create();

    // Setup mock - when CreateBuyOrder is called, return the BuyOrder that was passed in
    _mockRepository
        .Setup(x => x.CreateBuyOrder(It.IsAny<BuyOrder>()))
        .ReturnsAsync((BuyOrder bo) => bo);

    // Act
    BuyOrderResponse response = await _stocksService.CreateBuyOrder(request);

    // Assert
    response.Should().NotBeNull();
    response.BuyOrderID.Should().NotBe(Guid.Empty);
    response.StockSymbol.Should().Be("AAPL");
    response.StockName.Should().Be("Apple Inc.");
    response.Quantity.Should().Be(100);
    response.Price.Should().Be(150.00);
    response.TradeAmount.Should().Be(15000.00);

    // Verify the repository was called once
    _mockRepository.Verify(x => x.CreateBuyOrder(It.IsAny<BuyOrder>()), Times.Once);
}

    #endregion

    #region CreateSellOrder Tests

    [Fact]
    public async Task CreateSellOrder_NullRequest_ThrowsArgumentNullException()
{
    // Arrange
    SellOrderRequest? request = null;
    Func<Task> act = async () => await _stocksService.CreateSellOrder(request);

    // Act & Assert
    await act.Should().ThrowAsync<ArgumentNullException>();
}

[Fact]
public async Task CreateSellOrder_QuantityIsZero_ThrowsArgumentException()
{
    // Arrange
    SellOrderRequest request = _fixture.Build<SellOrderRequest>()
        .With(x => x.Quantity, (uint)0)
        .Create();
    
    // Act
    Func<Task> act = async () => await _stocksService.CreateSellOrder(request);
    
    // Assert
    await act.Should().ThrowAsync<ArgumentException>();
}

[Fact]
public async Task CreateSellOrder_QuantityExceedsMaximum_ThrowsArgumentException()
{
    // Arrange
    SellOrderRequest request = _fixture.Build<SellOrderRequest>()
        .With(x => x.Quantity, (uint)100001)
        .Create();
    Func<Task> act = async () => await _stocksService.CreateSellOrder(request);
    
    // Act & Assert
    await act.Should().ThrowAsync<ArgumentException>();
}

[Fact]
public async Task CreateSellOrder_PriceIsZero_ThrowsArgumentException()
{
    // Arrange
    SellOrderRequest request = _fixture.Build<SellOrderRequest>()
        .With(x => x.Quantity, (uint)50)
        .With(x => x.Price, 0)
        .Create();
    Func<Task> act = async () => await _stocksService.CreateSellOrder(request);
    
    // Act & Assert
    await act.Should().ThrowAsync<ArgumentException>();
}

[Fact]
public async Task CreateSellOrder_PriceExceedsMaximum_ThrowsArgumentException()
{
    // Arrange
    SellOrderRequest request = _fixture.Build<SellOrderRequest>()
        .With(x => x.Quantity, (uint)50)
        .With(x => x.Price, 10001)
        .Create();
    Func<Task> act = async () => await _stocksService.CreateSellOrder(request);
    
    // Act & Assert
    await act.Should().ThrowAsync<ArgumentException>();
}

[Fact]
public async Task CreateSellOrder_StockSymbolIsNull_ThrowsArgumentException()
{
    // Arrange
    SellOrderRequest request = _fixture.Build<SellOrderRequest>()
        .With(x => x.StockSymbol, (string?)null)
        .With(x => x.Quantity, (uint)50)
        .With(x => x.Price, 200.00)
        .Create();
    Func<Task> act = async () => await _stocksService.CreateSellOrder(request);
    
    // Act & Assert
    await act.Should().ThrowAsync<ArgumentException>();
}

[Fact]
public async Task CreateSellOrder_DateBeforeYear2000_ThrowsArgumentException()
{
    // Arrange
    SellOrderRequest request = _fixture.Build<SellOrderRequest>()
        .With(x => x.DateAndTimeOfOrder, new DateTime(1999, 12, 31))
        .With(x => x.Quantity, (uint)50)
        .With(x => x.Price, 200.00)
        .Create();
    Func<Task> act = async () => await _stocksService.CreateSellOrder(request);
    
    // Act & Assert
    await act.Should().ThrowAsync<ArgumentException>();
}

[Fact]
public async Task CreateSellOrder_ValidRequest_ReturnsSellOrderResponse()
{
    // Arrange
    SellOrderRequest request = _fixture.Build<SellOrderRequest>()
        .With(x => x.StockSymbol, "TSLA")
        .With(x => x.StockName, "Tesla Inc.")
        .With(x => x.DateAndTimeOfOrder, new DateTime(2024, 2, 10))
        .With(x => x.Quantity, (uint)75)
        .With(x => x.Price, 250.00)
        .Create();

    // Setup mock - when CreateSellOrder is called, return the SellOrder that was passed in
    _mockRepository
        .Setup(x => x.CreateSellOrder(It.IsAny<SellOrder>()))
        .ReturnsAsync((SellOrder so) => so);

    // Act
    SellOrderResponse response = await _stocksService.CreateSellOrder(request);

    // Assert
    response.Should().NotBeNull();
    response.SellOrderID.Should().NotBe(Guid.Empty);
    response.StockSymbol.Should().Be("TSLA");
    response.StockName.Should().Be("Tesla Inc.");
    response.DateAndTimeOfOrder.Should().Be(new DateTime(2024, 2, 10));
    response.Quantity.Should().Be(75);
    response.Price.Should().Be(250.00);
    response.TradeAmount.Should().Be(18750.00);  // 75 × 250 = 18,750

    // Verify the repository was called once
    _mockRepository.Verify(x => x.CreateSellOrder(It.IsAny<SellOrder>()), Times.Once);
}

    #endregion

    #region GetAllBuyOrders Tests

    [Fact]
    public async Task GetAllBuyOrders_ReturnsListOfBuyOrderResponse()
    {
        // Arrange
        List<BuyOrder> buyOrders = new List<BuyOrder>
        {
            _fixture.Build<BuyOrder>()
                .With(x => x.StockSymbol, "AAPL")
                .With(x => x.StockName, "Apple Inc.")
                .With(x => x.DateAndTimeOfOrder, DateTime.Now)
                .With(x => x.Quantity, (uint)100)
                .With(x => x.Price, 150.00)
                .Create(),
            _fixture.Build<BuyOrder>()
                .With(x => x.StockSymbol, "MSFT")
                .With(x => x.StockName, "Microsoft Corp.")
                .With(x => x.DateAndTimeOfOrder, DateTime.Now)
                .With(x => x.Quantity, (uint)50)
                .With(x => x.Price, 300.00)
                .Create()
        };

        // Setup mock - when GetBuyOrders is called, return the list
        _mockRepository
            .Setup(x => x.GetBuyOrders())
            .ReturnsAsync(buyOrders);

        // Act
        List<BuyOrderResponse> result = await _stocksService.GetAllBuyOrders();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result[0].StockSymbol.Should().Be("AAPL");
        result[0].TradeAmount.Should().Be(15000.00);  // 100 × 150
        result[1].StockSymbol.Should().Be("MSFT");
        result[1].TradeAmount.Should().Be(15000.00);  // 50 × 300

        // Verify the repository was called once
        _mockRepository.Verify(x => x.GetBuyOrders(), Times.Once);
    }

    [Fact]
    public async Task GetAllBuyOrders_EmptyList_ReturnsEmptyList()
    {
        // Arrange
        List<BuyOrder> emptyList = new List<BuyOrder>();

        // Setup mock - return empty list
        _mockRepository
            .Setup(x => x.GetBuyOrders())
            .ReturnsAsync(emptyList);

        // Act
        List<BuyOrderResponse> result = await _stocksService.GetAllBuyOrders();

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();

        // Verify the repository was called once
        _mockRepository.Verify(x => x.GetBuyOrders(), Times.Once);
    }

    #endregion

    #region GetAllSellOrders Tests

    [Fact]
    public async Task GetAllSellOrders_ReturnsListOfSellOrderResponse()
    {
        // Arrange
        List<SellOrder> sellOrders = new List<SellOrder>
        {
            _fixture.Build<SellOrder>()
                .With(x => x.StockSymbol, "TSLA")
                .With(x => x.StockName, "Tesla Inc.")
                .With(x => x.DateAndTimeOfOrder, DateTime.Now)
                .With(x => x.Quantity, (uint)75)
                .With(x => x.Price, 250.00)
                .Create(),
            _fixture.Build<SellOrder>()
                .With(x => x.StockSymbol, "GOOGL")
                .With(x => x.StockName, "Alphabet Inc.")
                .With(x => x.DateAndTimeOfOrder, DateTime.Now)
                .With(x => x.Quantity, (uint)25)
                .With(x => x.Price, 150.00)
                .Create()
        };

        // Setup mock - when GetSellOrders is called, return the list
        _mockRepository
            .Setup(x => x.GetSellOrders())
            .ReturnsAsync(sellOrders);

        // Act
        List<SellOrderResponse> result = await _stocksService.GetAllSellOrders();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result[0].StockSymbol.Should().Be("TSLA");
        result[0].TradeAmount.Should().Be(18750.00);  // 75 × 250
        result[1].StockSymbol.Should().Be("GOOGL");
        result[1].TradeAmount.Should().Be(3750.00);   // 25 × 150

        // Verify the repository was called once
        _mockRepository.Verify(x => x.GetSellOrders(), Times.Once);
    }

    [Fact]
    public async Task GetAllSellOrders_EmptyList_ReturnsEmptyList()
    {
        // Arrange
        List<SellOrder> emptyList = new List<SellOrder>();

        // Setup mock - return empty list
        _mockRepository
            .Setup(x => x.GetSellOrders())
            .ReturnsAsync(emptyList);

        // Act
        List<SellOrderResponse> result = await _stocksService.GetAllSellOrders();

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();

        // Verify the repository was called once
        _mockRepository.Verify(x => x.GetSellOrders(), Times.Once);
    }

    #endregion
}