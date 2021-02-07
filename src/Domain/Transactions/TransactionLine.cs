using MyWarehouse.Domain.Common;
using MyWarehouse.Domain.Common.ValueObjects.Money;
using MyWarehouse.Domain.Products;
using System.ComponentModel.DataAnnotations;

namespace MyWarehouse.Domain.Transactions
{
    /// <summary>
    /// Simplified entity. In production context it would probably record more momentary data,
    /// including partner name, address, etc., for reporting and historical purposes.
    /// </summary>
    public class TransactionLine : IEntity
    {
        internal TransactionLine() { }

        public int Id { get; private set; }

        [Required]
        public int ProductId { get; init; }
        public virtual Product Product { get; init; }

        [Required]
        public Transaction Transaction { get; init; }

        [Range(1, int.MaxValue)]
        public int Quantity { get; init; }

        [Required]
        public Money UnitPrice { get; init; }
    }
}