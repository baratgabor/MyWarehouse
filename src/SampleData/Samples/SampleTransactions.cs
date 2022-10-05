using MyWarehouse.Domain.Partners;
using MyWarehouse.Domain.Products;
using MyWarehouse.Domain.Transactions;

namespace MyWarehouse.TestData.Samples;

internal static class SampleTransactions
{
    // Determines the minimum & maximum number of transaction lines to include in each transaction.
    private const int MinTransactionLines = 1;
    private const int MaxTransactionLines = 8;
    
    // Determines the minimum & maximum number of products to include in each transaction line.
    private const int MinProcurementQuantity = 1;
    private const int MaxProcurementQuantity = 30;

    // Determines the maximum ratio of stock to sell for any given product while creating a sales transaction line.
    private const float MaximumSellingRatio = 0.4f;

    // Determines how many products to procure initially before starting to sell.
    private const int StockPreloadProductCount = 15;

    private static readonly Random _rnd = new(21395443);

    internal static Transaction GenerateTransaction(IReadOnlyList<Partner> partners, IReadOnlyList<Product> allProducts)
    {
        var availableProducts = allProducts.Where(x => x.NumberInStock > 0);
        var unavailableProducts = allProducts.Where(x => x.NumberInStock == 0);

        var shouldBeProcurement = availableProducts.Count() < StockPreloadProductCount || _rnd.Next(0, 2) == 0;
        var randomPartner = partners[_rnd.Next(0, partners.Count)];

        Func<IEnumerable<(Product, int)>, Transaction> transactionMethod = shouldBeProcurement 
            ? randomPartner.ProcureFrom 
            : randomPartner.SellTo;

        var productSelection = shouldBeProcurement 
            ? unavailableProducts.Any()
                ? unavailableProducts
                : allProducts
            : availableProducts;
        var productSelectionCount = productSelection.Count();

        // Select some random products, and project them to transaction lines with random quantities based on the transaction type.
        var transactionLines = productSelection
            .SelectRandom(_rnd.Next(MinTransactionLines, MaxTransactionLines + 1))
            .Select(p => (p, _rnd.Next(MinProcurementQuantity, shouldBeProcurement
                ? MaxProcurementQuantity + 1
                : Math.Max(MinProcurementQuantity, (int)(p.NumberInStock * MaximumSellingRatio)))));

        return transactionMethod(transactionLines);
    }

    internal static IEnumerable<T> SelectRandom<T>(this IEnumerable<T> list, int needed)
    {
        var count = list.Count();
        if (needed >= count)
            return list;

        var selectedItems = new HashSet<T>();
        while (needed > 0)
            if (selectedItems.Add(list.ElementAt(_rnd.Next(count))))
                needed--;

        return selectedItems;
    }
}
