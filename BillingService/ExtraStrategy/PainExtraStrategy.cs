namespace BillingService.ExtraStrategy
{
    public class PainExtraStrategy : ExtraStrategy,IExtraCalculationStrategy
    {
        public PainExtraStrategy(decimal extraPrice)
        {
            _extraPrice = extraPrice;
        }
    }
}
