namespace BillingService.ExtraStrategy
{
    public interface IExtraCalculationStrategy
    {
        decimal CalculateExtra(int quantity);
    }
}
