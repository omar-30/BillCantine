namespace BillingService.ExtraStrategy
{
    public class EntreeExtraStrategy : ExtraStrategy,IExtraCalculationStrategy
    {
        public EntreeExtraStrategy(decimal extraPrice)
        {
            _extraPrice = extraPrice;
        }
    }
}
