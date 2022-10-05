namespace MyWarehouse.Application.Products.ProductStockValue;

public record StockValueDto
{
    public decimal Amount { get; init; }
    public string CurrencyCode { get; init; } = null!;
}
