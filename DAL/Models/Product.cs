namespace DAL.Models;

public class Product
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required ProductType Type { get; set; }
    public decimal Price { get; set; }

    public bool IsTypeNotInPlateFixed { 
        get 
        {
            return Type == ProductType.Autre; 
        }
    }
}