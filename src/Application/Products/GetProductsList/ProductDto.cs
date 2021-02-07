using MyWarehouse.Application.Common.Mapping;
using MyWarehouse.Domain.Products;

namespace MyWarehouse.Application.Products.GetProducts
{
    public record ProductDto : IMapFrom<Product>
    {
        public int Id { get; init; }

        public string Name { get; init; }
        public string Description { get; init; }

        public decimal PriceAmount { get; init; }
        public string PriceCurrencyCode { get; init; }

        public float MassValue { get; init; }
        public string MassUnitSymbol { get; init; }

        public int NumberInStock { get; init; }
    }
}
