using MyWarehouse.Domain.Common.ValueObjects.Mass;
using MyWarehouse.Domain.Common.ValueObjects.Money;
using MyWarehouse.Domain.Partners;
using MyWarehouse.Domain.Products;
using MyWarehouse.Domain.Transactions;
using System.Linq;
using System.Reflection;

namespace MyWarehouse.Domain.UnitTests
{
    internal static class TestHelper
    {
        public static Address NewValidAddress
            => new Address("123 Street", "Berlin", "Germany", "123456");

        public static Product NewValidProduct_NoStock
            => new Product("Product", "Description", new Money(44.99m, Currency.USD), new Mass(2.5f, MassUnit.Kilogram));

        public static Product NewValidProduct_WithStock(int stock)
            => AddStockToProduct(NewValidProduct_NoStock, stock);

        public static Partner NewValidPartner
            => new Partner("PartnerName", NewValidAddress);

        public static Transaction NewTransaction_EmptyProcurement
            => new Transaction(TransactionType.Procurement, NewValidPartner);

        public static Transaction NewTransaction_EmptySales
            => new Transaction(TransactionType.Sales, NewValidPartner);

        private static Product AddStockToProduct(Product product, int stock)
        {
            var fi = product.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance).Where(fi => fi.Name.Contains(nameof(Product.NumberInStock))).Single();
            fi.SetValue(product, stock);
            return product;
        }
    }
}
