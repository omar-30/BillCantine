using DAL.Models;
namespace BillingService.ExtraStrategy
{
    public class ExtraCalculationStrategyFactory
    {
        private EntreeExtraStrategy entreeExtraStrategy { get; } = new EntreeExtraStrategy(3);
        private DessertExtraStrategy dessertExtraStrategy { get; } = new DessertExtraStrategy(3);
        private PlatExtraStrategy platExtraStrategy { get; } = new PlatExtraStrategy(6);
        private PainExtraStrategy painExtraStrategy { get; } = new PainExtraStrategy(0.4m);

        public ExtraCalculationStrategyFactory() { }

        public IExtraCalculationStrategy GetStrategy(ProductType productType)
        {
            return productType switch
            {
                ProductType.Entree => entreeExtraStrategy,
                ProductType.Plat => platExtraStrategy,
                ProductType.Dessert => dessertExtraStrategy,
                ProductType.Pain => painExtraStrategy,
                ProductType.Autre => throw new NotImplementedException(),
                _ => throw new NotImplementedException()
            };
        }
    }
}
