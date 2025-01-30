namespace Api.Requests
{
    public class TicketRequest
    {
        public int ClientId { get; set; } = new();
        public Dictionary<int,int> OrderProducts { get; set; } = new();
    }
}
