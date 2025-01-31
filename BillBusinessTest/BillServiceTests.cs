using BillingManagement.Services;
using DAL;
using DAL.Models;
using Microsoft.EntityFrameworkCore;

public class BillServiceTests
{
    private readonly BillService _billService;
    private readonly CantineDbContext _dbContext;

    public CantineDbContext DbContext => _dbContext;

    public BillServiceTests()
    {
        var options = new DbContextOptionsBuilder<CantineDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDb")
            .Options;

        _dbContext = new CantineDbContext(options);
        _dbContext.Database.EnsureDeleted();
        _dbContext.Database.EnsureCreated();

        _billService = new BillService(_dbContext);
    }


    [Fact]
    public void GenerateTicket_FixedPlateWithExtra_CalculatesCorrectTotal()
    {
        var order = new Dictionary<int, int>
        {
            { 1, 2 },  // 2x Entrées
            { 2, 1 },  // 1x Plat
            { 3, 2 },  // 2x Desserts
            { 5, 1 }   // 1x Pain
        };

        var ticket = _billService.GenerateTicket(1, order);

        Assert.NotNull(ticket);
        Assert.Equal(10m + 3 + 3, ticket.TotalPlate); // Plat fixe + Extra (Entrée: 3 + Dessert: 3)
        Assert.Equal(200m - ticket.AmountToPay, _dbContext.Clients.First(c => c.Id == 1).Balance);
    }

    [Fact]
    public void GenerateTicket_FixedPlateWithExtraPlat_CalculatesCorrectTotal()
    {
        var order = new Dictionary<int, int>
        {
            { 1, 1 },  // 1x Entrée
            { 2, 2 },  // 2x Plats
            { 3, 1 },  // 1x Dessert
            { 5, 1 }   // 1x Pain
        };

        var ticket = _billService.GenerateTicket(1, order);

        Assert.NotNull(ticket);
        Assert.Equal(10m + 6, ticket.TotalPlate); // Plat fixe + Extra Plat (6)
        Assert.Equal(200m - ticket.AmountToPay, _dbContext.Clients.First(c => c.Id == 1).Balance);
    }

    [Fact]
    public void GenerateTicket_FixedPlateWithExtraAndNonFixedItem_CalculatesCorrectTotal()
    {
        var order = new Dictionary<int, int>
        {
            { 1, 1 },  // 1x Entrée
            { 2, 1 },  // 1x Plat
            { 3, 1 },  // 1x Dessert
            { 5, 1 },  // 1x Pain
            { 6, 2 }   // 2x Boissons (hors plat fixe)
        };

        var ticket = _billService.GenerateTicket(1, order);

        Assert.NotNull(ticket);
        Assert.Equal(10m + 2 , ticket.TotalPlate); // Plat fixe + 2x Boissons (hors plat fixe)
        Assert.Equal(200m - ticket.AmountToPay, _dbContext.Clients.First(c => c.Id == 1).Balance);
    }

    [Fact]
    public void GenerateTicket_FixedPlateWithExtraPainAndNonFixedItem_CalculatesCorrectTotal()
    {
        var order = new Dictionary<int, int>
        {
            { 1, 1 },  // 1x Entrée
            { 2, 1 },  // 1x Plat
            { 3, 1 },  // 1x Dessert
            { 5, 2 },  // 2x Pain (1 pain hors plat fixe)
            { 6, 2 }   // 2x Boissons (hors plat fixe)
        };

        var ticket = _billService.GenerateTicket(1, order);

        Assert.NotNull(ticket);
        Assert.Equal(10m + 2 + 0.4m, ticket.TotalPlate); // Plat fixe + 2x Boissons (hors plat fixe) + 1 pain (hors plat fixe) 
        Assert.Equal(200m - ticket.AmountToPay, _dbContext.Clients.First(c => c.Id == 1).Balance);
    }


    [Fact]
    public void GenerateTicket_NoFixedPlate_CalculatesCorrectTotal()
    {
        var order = new Dictionary<int, int>
        {
            { 1, 1 },  // 1x Entrée
            { 3, 1 },  // 1x Dessert
            { 6, 1 }   // 1x Boisson
        };

        var ticket = _billService.GenerateTicket(1, order);

        Assert.NotNull(ticket);
        Assert.Equal(4m + 1m + 1m, ticket.TotalPlate); // Entrée + Dessert + Boisson
        Assert.Equal(200m - ticket.AmountToPay, _dbContext.Clients.First(c => c.Id == 1).Balance);
    }


    [Fact]
    public void GetClient_ExistingClient_ReturnsClient()
    {
        var client = _billService.GetClient(1);
        Assert.NotNull(client);
        Assert.Equal(1, client.Id);
    }

    [Fact]
    public void GetClient_NonExistingClient_ThrowsException()
    {
        Assert.Throws<ArgumentException>(() => _billService.GetClient(99));
    }


    [Fact]
    public void GenerateTicket_ValidOrder_ReturnsTicket()
    {
        var order = new Dictionary<int, int> { { 1, 1 }, { 2, 1 }, { 3, 1 }, { 5, 1 } }; // Entrée, Plat, Dessert, Pain
        var ticket = _billService.GenerateTicket(1, order);

        Assert.NotNull(ticket);
        Assert.Equal(1, ticket.ClientId);
        Assert.Equal(10m, ticket.TotalPlate); // Prix du plat fixe
        Assert.Equal(200m - ticket.AmountToPay, DbContext.Clients.First(c => c.Id == 1).Balance);
    }

    [Fact]
    public void GenerateTicket_InsufficientBalance_ThrowsException()
    {
        var client = DbContext.Clients.First(c => c.Id == 2);

        var order = new Dictionary<int, int> { { 1, 1 }, { 2, 1 }, { 3, 1 }, { 4, 1 } };

        Assert.Throws<InvalidOperationException>(() => _billService.GenerateTicket(2, order));
    }

    [Fact]
    public void GenerateTicket_VIP_AmountToPayZero()
    {
        var client = DbContext.Clients.First(c => c.Id == 3);

        var order = new Dictionary<int, int> { { 1, 1 }, { 2, 1 }, { 3, 1 }, { 4, 1 } };

        var ticket = _billService.GenerateTicket(3, order);

        Assert.Equal(0, ticket.AmountToPay);
    }

    [Fact]
    public void GenerateTicket_Internal_AllowNegativeBalance()
    {
        var client = DbContext.Clients.First(c => c.Id == 5);

        var order = new Dictionary<int, int> { { 1, 1 }, { 2, 1 }, { 3, 1 }, { 5, 1 } };

        var ticket = _billService.GenerateTicket(5, order);

        Assert.Equal(2.5m, ticket.AmountToPay);
        Assert.Equal(-0.5m, client.Balance);
    }


    [Fact]
    public void CreditBalance_PositiveAmount_UpdatesBalance()
    {
        var client = DbContext.Clients.First(c => c.Id == 1);
        Assert.Equal(200m, client.Balance);
        _billService.CreditBalance(1, 20m);
        client = DbContext.Clients.First(c => c.Id == 1);
        Assert.Equal(220m, client.Balance);
    }

    [Fact]
    public void CreditBalance_NegativeAmount_ThrowsException()
    {
        Assert.Throws<ArgumentException>(() => _billService.CreditBalance(1, -10m));
    }
}
