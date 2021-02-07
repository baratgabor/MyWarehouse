using MyWarehouse.Domain.Partners;
using MyWarehouse.Domain.Products;
using MyWarehouse.Domain.Transactions;
using MyWarehouse.TestData.Samples;
using System.Collections.Generic;

namespace MyWarehouse.TestData
{
    public static class DataGenerator
    {
        /// <summary>
        /// Generates Partner and Product entities.
        /// </summary>
        public static (List<Product>, List<Partner>) GenerateBaseEntities()
        {
            var products = SampleProducts.GenerateSampleProducts(76);
            var partners = SamplePartners.GetSamplePartners();
            
            return (products, partners);
        }

        /// <summary>
        /// Generates a single random transaction by selecting one partner, one or multiple products, and one or multiple quantity for each product.
        /// Note that transaction will be persisted at save even without using the return value, because transactions are created inside the Partner aggregate root.
        /// </summary>
        public static Transaction GenerateTransaction(IReadOnlyList<Partner> partners, IReadOnlyList<Product> products)
            => SampleTransactions.GenerateTransaction(partners, products);
    }
}