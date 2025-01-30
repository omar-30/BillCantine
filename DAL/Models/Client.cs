namespace DAL.Models
{
    public class Client
    {
        public int Id { get; set; }

        public required string Name { get; set; }

        public decimal Balance { get; set; }

        public ClientType Type { get; set; }

        public decimal Coverage
        {
            get
            {
                switch (Type)
                {
                    case ClientType.Internal:
                        return 7.5m;
                    case ClientType.Contractor:
                        return 6;
                    case ClientType.Intern:
                        return 10;
                    case ClientType.Visitor:
                        return 0;
                }
                // VIP client
                return decimal.MaxValue;
            }
        }

        public bool CheckCanPayWithBalance(decimal amountToPay)
        {
            return Type == ClientType.Internal || Type == ClientType.VIP || Balance >= amountToPay; 
        }
    }
}
