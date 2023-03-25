using MyWarehouse.Domain.Products;

namespace MyWarehouse.Domain.Exceptions;

public class InsufficientStockException : Exception
{
    public string ProductName { get; }
    public int RequestedQuantity { get; }
    public int ActualQuantity { get; }

    public InsufficientStockException(Product product, int requestedQuantity, int actualQuantity)
        : base($"Quantity requested for sale ({requestedQuantity}) from product '{product.Name}' exceeds number in stock ({actualQuantity}).")
    {
        ProductName = product.Name;
        RequestedQuantity = requestedQuantity;
        ActualQuantity = actualQuantity;
    }
}
