namespace MyWarehouse.Application.Products.GetProductsSummary
{
    public record ProductStockCountDto
    {
        public int ProductCount { get; init; }
        public int TotalStock { get; init; }
    }
}
