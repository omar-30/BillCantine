namespace DAL.Models
{
    public class ClientTicket
    {
        public int ClientId { get; set; }
        public List<Product> Products { get; set; } = new();
        public decimal TotalPlate { get; set; }
        public decimal AmountToPay { get; set; }
    }
}