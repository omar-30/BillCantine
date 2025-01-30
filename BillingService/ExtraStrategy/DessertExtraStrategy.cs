namespace BillingService.ExtraStrategy
{
    public class DessertExtraStrategy : ExtraStrategy, IExtraCalculationStrategy
    {
        public DessertExtraStrategy(int extraPrice) 
        {
            _extraPrice = extraPrice;
        }
    }
}
