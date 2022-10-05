using MyWarehouse.Domain.Common;
using MyWarehouse.Domain.Products;
using MyWarehouse.Domain.Transactions;
using System.Diagnostics.CodeAnalysis;

namespace MyWarehouse.Domain.Partners;

/// <summary>
/// Simplified entity. In product context a partner would contain more fine grained name fields,
/// a complex address representation, phone number, invoicing/tax details, etc.
/// </summary>
public class Partner : MyEntity
{
    [Required]
    [StringLength(PartnerInvariants.NameMaxLength)]
    public string Name { get; private set; }

    [Required]
    public Address Address { get; private set; }

    public virtual IReadOnlyCollection<Transaction> Transactions => _transactions.AsReadOnly();
    private List<Transaction> _transactions = new();

    private Partner() // EF
    {
        Name = default!;
        Address = default!;
    }

    public Partner(string name, Address address)
    {
        UpdateName(name);
        UpdateAddress(address);
    }

    /// <summary>
    /// Generate a new sales transaction with this partner.
    /// </summary>
    public Transaction SellTo(IEnumerable<(Product product, int quantity)> items)
        => CreateTransaction(items, TransactionType.Sales);

    /// <summary>
    /// Generate a new procurement transaction with this partner.
    /// </summary>
    public Transaction ProcureFrom(IEnumerable<(Product product, int quantity)> items)
        => CreateTransaction(items, TransactionType.Procurement);

    [MemberNotNull(nameof(Name))]
    public void UpdateName(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Name cannot be empty.");

        if (value.Length > PartnerInvariants.NameMaxLength)
            throw new ArgumentException($"Length of value ({value.Length}) exceeds maximum name length ({ProductInvariants.NameMaxLength}).");

        Name = value;
    }

    [MemberNotNull(nameof(Address))]
    public void UpdateAddress(Address address)
    {
        Address = address ?? throw new ArgumentNullException(nameof(address));
    }

    private Transaction CreateTransaction(IEnumerable<(Product product, int quantity)> items, TransactionType transactionType)
    {
        if (items == null)
            throw new ArgumentNullException(nameof(items));

        if (!items.Any() || items.Any(x => x.product == null || x.quantity < 1))
            throw new ArgumentException("List of items must be a non-empty list of non-null products and quantities of at least 1.", nameof(items));

        var transaction = new Transaction(
            type: transactionType,
            partner: this
        );

        foreach (var (product, quantity) in items)
        {
            transaction.AddTransactionLine(product, quantity);
        }

        _transactions.Add(transaction);

        return transaction;
    }
}
