using DAL.Models;

namespace BillingManagement.Services
{
    public interface IBillService
    {
        Client GetClient(int clientId);
        ClientTicket GenerateTicket(int clientId, Dictionary<int, int> orderProducts);
        void CreditBalance(int clientId, decimal amount);
    }
}