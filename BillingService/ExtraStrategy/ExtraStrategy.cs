namespace BillingService.ExtraStrategy
{
    public class ExtraStrategy : IExtraCalculationStrategy
    {
        protected bool isFirst = true;
        protected decimal _extraPrice = 0;

        public decimal CalculateExtra(int quantity)
        {
            var calculExtra = _extraPrice * (isFirst ? quantity - 1 : quantity);
            isFirst = false;
            return calculExtra;
        }
    }
}
