using MyWarehouse.Application.Common.Mapping;
using MyWarehouse.Domain.Products;
using System.Diagnostics.CodeAnalysis;

namespace MyWarehouse.Application.Products.GetProduct;

public record ProductDetailsDto : IMapFrom<Product>
{
    public int Id { get; init; }

    public string Name { get; init; } = null!;
    public string Description { get; init; } = null!;

    public DateTime CreatedAt { get; init; }
    public string CreatedBy { get; init; } = null!;

    public DateTime? LastModifiedAt { get; init; }
    public string? LastModifiedBy { get; init; }

    public decimal PriceAmount { get; init; }
    public string PriceCurrencyCode { get; init; } = null!;

    public float MassValue { get; init; }
    public string MassUnitSymbol { get; init; } = null!;

    public int NumberInStock { get; init; }
}
