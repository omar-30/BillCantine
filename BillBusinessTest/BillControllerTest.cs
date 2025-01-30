using System.Collections.Generic;
using System.Threading.Tasks;
using Api.Requests;
using BillingManagement.Services;
using DAL.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using Cantine.Controllers;

public class BillControllerUnitTests
{
    private readonly Mock<IBillService> _billServiceMock;
    private readonly Mock<ILogger<BillController>> _loggerMock;
    private readonly BillController _controller;

    public BillControllerUnitTests()
    {
        _billServiceMock = new Mock<IBillService>();
        _loggerMock = new Mock<ILogger<BillController>>();
        _controller = new BillController(_billServiceMock.Object, _loggerMock.Object);
    }

    [Fact]
    public void GetBalance_ValidClient_ReturnsOk()
    {
        var client = new Client { Id = 1, Name = "Alice", Balance = 50m };
        _billServiceMock.Setup(s => s.GetClient(1)).Returns(client);

        var result = _controller.GetBalance(1) as OkObjectResult;

        Assert.NotNull(result);
        Assert.Equal(200, result.StatusCode);

        dynamic response = result.Value;
        Assert.Equal("{ clientId = 1, Balance = 50 }", response.ToString());
    }

    [Fact]
    public void GetBalance_InvalidClient_ReturnsBadRequest()
    {
        _billServiceMock.Setup(s => s.GetClient(999)).Throws(new System.ArgumentException("le client n'existe pas"));

        var result = _controller.GetBalance(999) as BadRequestObjectResult;

        Assert.NotNull(result);
        Assert.Equal(400, result.StatusCode);
    }

    [Fact]
    public void CreditBalance_ValidRequest_ReturnsOk()
    {
        var request = new CreditRequest { ClientId = 1, Amount = 20m };

        var result = _controller.CreditBalance(request) as OkObjectResult;

        Assert.NotNull(result);
        Assert.Equal(200, result.StatusCode);
    }

    [Fact]
    public void CreditBalance_InvalidClient_ReturnsBadRequest()
    {
        var request = new CreditRequest { ClientId = 999, Amount = 20m };
        _billServiceMock.Setup(s => s.CreditBalance(999, 20m)).Throws(new System.ArgumentException("le client n'existe pas"));

        var result = _controller.CreditBalance(request) as BadRequestObjectResult;

        Assert.NotNull(result);
        Assert.Equal(400, result.StatusCode);
    }

    [Fact]
    public void GenerateTicket_ValidRequest_ReturnsOk()
    {
        var request = new TicketRequest
        {
            ClientId = 1,
            OrderProducts = new Dictionary<int, int> { { 1, 1 }, { 2, 1 } }
        };

        var ticket = new ClientTicket { ClientId = 1, TotalPlate = 10m, AmountToPay = 5m };
        _billServiceMock.Setup(s => s.GenerateTicket(1, request.OrderProducts)).Returns(ticket);

        var result = _controller.GenerateTicket(request) as OkObjectResult;

        Assert.NotNull(result);
        Assert.Equal(200, result.StatusCode);
        Assert.Equal(ticket, result.Value);
    }

    [Fact]
    public void GenerateTicket_InvalidClient_ReturnsBadRequest()
    {
        var request = new TicketRequest { ClientId = 999, OrderProducts = new Dictionary<int, int> { { 1, 1 } } };
        _billServiceMock.Setup(s => s.GenerateTicket(999, request.OrderProducts)).Throws(new System.ArgumentException("le client n'existe pas"));

        var result = _controller.GenerateTicket(request) as BadRequestObjectResult;

        Assert.NotNull(result);
        Assert.Equal(400, result.StatusCode);
    }
}
