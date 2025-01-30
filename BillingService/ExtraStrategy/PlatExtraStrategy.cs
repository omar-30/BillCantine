namespace BillingService.ExtraStrategy
{
    public class PlatExtraStrategy : ExtraStrategy,IExtraCalculationStrategy
    {
        public PlatExtraStrategy(int extraPrice)
        {
            _extraPrice = extraPrice;
        }
    }
}
