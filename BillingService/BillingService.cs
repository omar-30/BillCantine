using BillingService.ExtraStrategy;
using DAL;
using DAL.Models;

namespace BillingManagement.Services;

public class BillService : IBillService
{
    private readonly CantineDbContext _context;

    private const decimal FixedPlatePrice = 10m;

    public BillService(CantineDbContext context)
    {
        _context = context;
    }

    public Client GetClient(int clientId)
    {
        var client = _context.Clients.FirstOrDefault(c => c.Id == clientId);
        if (client == null)
        {
            throw new ArgumentException("le client n'existe pas");
        }

        return client;
    }


    public ClientTicket GenerateTicket(int clientId, Dictionary<int,int> productItems)
    {
        var client = _context.Clients.FirstOrDefault(c => c.Id == clientId);
        if (client == null)
        {
            throw new ArgumentException("le client n'existe pas");
        }

        var ordredProducts = _context.Products.Where(c => productItems.Keys.Contains(c.Id)).ToList();
        if (ordredProducts.Count != productItems.Count)
        {
            throw new ArgumentException("Un ou plusieurs produits n'existe pas");
        }

        var ticket = new ClientTicket { ClientId = client.Id, Products = ordredProducts };

        // check if the order contains a fixed plate 
        var isFixedPlate = IsFixedPlate(ordredProducts);
        if (isFixedPlate)
        {
            ticket.TotalPlate = FixedPlatePrice;
        }

        //add Extra menu
        ticket.TotalPlate += AddExtra(ordredProducts, productItems, isFixedPlate);

        // Calculer la prise en charge employeur
        var employerCoverage = Math.Min(client.Coverage, ticket.TotalPlate);

        ticket.AmountToPay = ticket.TotalPlate - employerCoverage;

        // valider le paiement pour les clients non autorisés au découvert
        if (!client.CheckCanPayWithBalance(ticket.AmountToPay))
        {
            throw new InvalidOperationException($"Solde insuffisant {client.Balance} pour le paiement de {ticket.AmountToPay}.");
        }

        client.Balance -= ticket.AmountToPay;

        _context.SaveChanges();

        return ticket;
    }
    /// <summary>
    /// ajouter extra avec les regles de supplement dessert:3, plat:6,Entree:3
    /// </summary>
    /// <param name="ordredProducts"></param>
    /// <param name="orderProducts"></param>
    /// <param name="isWithFixedPlate"></param>
    /// <returns></returns>
    private decimal AddExtra(List<Product> ordredProducts, Dictionary<int, int> orderProducts, bool isWithFixedPlate = true)
    {
        var total = 0.0m;
        
        var factory = new ExtraCalculationStrategyFactory();

        foreach (var product in ordredProducts) 
        {
            int quantity = orderProducts[product.Id];

            if (product.IsTypeNotInPlateFixed || !isWithFixedPlate)
            {
                total += product.Price * quantity;
            }
            else
            {
                var strategy = factory.GetStrategy(product.Type);
                total += strategy.CalculateExtra(quantity);     
            }    
        }
        return total;
    }

    /// <summary>
    /// check si le plat correspond au plat fixe
    /// </summary>
    /// <param name="products"></param>
    /// <returns></returns>
    private bool IsFixedPlate(List<Product> products)
    {
        return products.Count(p => p.Type == ProductType.Plat) >= 1 &&
               products.Count(p => p.Type == ProductType.Pain) >= 1 &&
               products.Count(p => p.Type == ProductType.Dessert) >= 1 &&
               products.Count(p => p.Type == ProductType.Entree) >= 1;
    }
    
    /// <summary>
    /// charger la balance client 
    /// </summary>
    /// <param name="clientId"></param>
    /// <param name="amount"></param>
    /// <exception cref="ArgumentException"></exception>
    public void CreditBalance(int clientId, decimal amount)
    {
        var client = _context.Clients.FirstOrDefault(c => c.Id == clientId);
        if (client == null)
        {
            throw new ArgumentException("le client n'existe pas");
        }

        if (amount <= 0)
        {
            throw new ArgumentException("Le montant doit être positif.");
        }

        client.Balance += amount;
        _context.SaveChanges();
    }
}