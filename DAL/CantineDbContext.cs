using DAL.Models;
using Microsoft.EntityFrameworkCore;
namespace DAL;

public class CantineDbContext : DbContext
{
    public DbSet<Client> Clients { get; set; }
    public DbSet<Product> Products { get; set; }

    public CantineDbContext(DbContextOptions<CantineDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Client>().HasData(
            new Client { Id = 1, Name = "Dupont", Type = ClientType.Internal, Balance = 200 },
            new Client { Id = 2, Name = "Jean", Type = ClientType.Visitor, Balance = 1 },
            new Client { Id = 3, Name = "Paul", Type = ClientType.VIP, Balance = 0 },
            new Client { Id = 4, Name = "Eric", Type = ClientType.Intern, Balance = 100 }
        );

        modelBuilder.Entity<Product>().HasData(
            new Product { Id = 6, Name = "Boisson", Type = ProductType.Autre, Price = 1 },
            new Product { Id = 1, Name = "Petit Salade Bar", Type = ProductType.Entree, Price = 4 },
            new Product { Id = 2, Name = "Grand Salade Bar", Type = ProductType.Plat, Price = 6 },
            new Product { Id = 3, Name = "Fromage", Type = ProductType.Dessert, Price = 1 },
            new Product { Id = 4, Name = "Portion de fruit", Type = ProductType.Dessert, Price = 1 },
            new Product { Id = 5, Name = "Pain", Type = ProductType.Pain, Price = 0.4m },
            new Product { Id = 7, Name = "Entree 1", Type = ProductType.Entree, Price = 3 },
            new Product { Id = 8, Name = "Plat 1", Type = ProductType.Plat, Price = 6 },
            new Product { Id = 9, Name = "Dessert 1", Type = ProductType.Dessert, Price = 3 }
        );
    }
}
