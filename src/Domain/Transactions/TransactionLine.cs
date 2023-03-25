using MyWarehouse.Domain.Common;
using MyWarehouse.Domain.Common.ValueObjects.Money;
using MyWarehouse.Domain.Products;

namespace MyWarehouse.Domain.Transactions;

/// <summary>
/// Simplified entity. In production context it would probably record more momentary data,
/// including partner name, address, etc., for reporting and historical purposes.
/// </summary>
public class TransactionLine : IEntity
{
    public int Id { get; private set; }

    [Required]
    public int ProductId { get; init; }
    public virtual Product Product { get; init; } = null!;

    [Required]
    public Transaction Transaction { get; init; } = null!;

    [Range(1, int.MaxValue)]
    public int Quantity { get; init; }

    [Required]
    public Money UnitPrice { get; init; } = null!;

    internal TransactionLine() { }
}
