using MyWarehouse.Application.Common.Mapping;
using MyWarehouse.Domain.Products;

namespace MyWarehouse.Application.Products.GetProducts;

public record ProductDto : IMapFrom<Product>
{
    public int Id { get; init; }

    public string Name { get; init; } = null!;
    public string Description { get; init; } = null!;
    
    public decimal PriceAmount { get; init; }
    public string PriceCurrencyCode { get; init; } = null!;

    public float MassValue { get; init; }
    public string MassUnitSymbol { get; init; } = null!;

    public int NumberInStock { get; init; }
}
