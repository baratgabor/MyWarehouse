namespace MyWarehouse.Application.Products.ProductStockMass
{
    public record StockMassDto
    {
        public float Value { get; init; }
        public string Unit { get; init; }
    }
}
