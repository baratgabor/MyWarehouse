using MyWarehouse.Domain.Common;
using MyWarehouse.Domain.Common.ValueObjects.Money;
using MyWarehouse.Domain.Partners;
using MyWarehouse.Domain.Products;

namespace MyWarehouse.Domain.Transactions;

/// <summary>
/// Represents inventory movement, with a total value calculated as
/// inventory movement total as the time of the transaction.
/// Does not model traditional purchase and sales orders.
/// </summary>
public class Transaction : MyEntity
{
    public TransactionType TransactionType { get; private set; }

    [Required]
    public Money Total { get; private set; } = new Money(0, Currency.USD);

    public int PartnerId { get; private set; }
    public virtual Partner Partner { get; private set; }

    public virtual IReadOnlyCollection<TransactionLine> TransactionLines => _transactionLines.AsReadOnly();
    private readonly List<TransactionLine> _transactionLines = new();

    private Transaction() // EF
    {
        Partner = null!;
    }

    internal Transaction(TransactionType type, Partner partner)
    {
        TransactionType = type;
        Partner = partner;
    }

    internal void AddTransactionLine(Product product, int quantity)
    {
        if (product == null)
            throw new ArgumentNullException(nameof(product));

        if (quantity < 1)
            throw new ArgumentException("Value must be equal to or greater than 1.", nameof(quantity));

        // Sales quantity vs Product stock validation is a Product responsibility; see RecordTransaction().

        var transactionLine = new TransactionLine()
        {
            Transaction = this,
            Product = product,
            Quantity = quantity,
            UnitPrice = product.Price.Copy()
        };

        product.RecordTransaction(transactionLine);

        _transactionLines.Add(transactionLine);

        var currency = _transactionLines.First().UnitPrice.Currency;
        Total = TransactionLines.Aggregate(new Money(0, currency),
            (total, line) => total + (line.UnitPrice * line.Quantity));
    }
}
