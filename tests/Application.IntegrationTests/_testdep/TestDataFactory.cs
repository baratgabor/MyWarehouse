using MyWarehouse.Domain.Common.ValueObjects.Mass;
using MyWarehouse.Domain.Common.ValueObjects.Money;
using MyWarehouse.Domain.Partners;
using MyWarehouse.Domain.Products;
using MyWarehouse.Domain.Transactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyWarehouse.Application.IntegrationTests
{
    public class TestDataFactory
    {
        private readonly TestData _data;

        public TestDataFactory(TestData data) => _data = data;

        /// <summary>
        /// Creates the requested number of products, and adds them to the database.
        /// </summary>
        /// <param name="productOverrides">Action to invoke on each product created, to allow property customization before saving.</param>
        /// <returns>The list of created products.</returns>
        public async Task<List<Product>> AddProducts(int howMany, Action<(int index, Product product)> productOverrides = null)
        {
            var products = new Product[howMany];
            for (int i = 0; i < howMany; i++)
            {
                var product = new Product(
                    name: "Product" + i,
                    description: "Description" + i,
                    price: new Money((i+1) * 10, Currency.USD),
                    mass: new Mass(i+1, MassUnit.Kilogram));

                productOverrides?.Invoke((i, product));
                products[i] = product;
            }

            await _data.AddRangeAsync(products);
            return products.ToList();
        }

        /// <summary>
        /// Creates the requested number of partners, and adds them to the database.
        /// </summary>
        /// <param name="partnerOverrides">Action to invoke on each partner created, to allow property customization before saving.</param>
        /// <returns>The list of created partners.</returns>
        public async Task<List<Partner>> AddPartners(int howMany, Action<(int index, Partner partner)> partnerOverrides = null)
        {
            var partners = new Partner[howMany];
            for (int i = 0; i < howMany; i++)
            {
                var partner = new Partner(
                    name: "Partner Number" + (i+1),
                    address: new Address(
                        country: "Canada",
                        city: "Toronto",
                        street: (i+1) + " Test street",
                        zipcode: string.Join("", Enumerable.Range(i+1, 6)).Substring(0, 6))
                    );

                partnerOverrides?.Invoke((i, partner));
                partners[i] = partner;
            }

            await _data.AddRangeAsync(partners);
            return partners.ToList();
        }

        /// <summary>
        /// Creates a procurement transaction.
        /// </summary>
        /// <param name="partner">The partner to create a transaction with. If not provided, a new partner will be created.</param>
        /// <param name="createNewProducts">If true, new products will be created for all transaction lines. If false, products will be read from database.</param>
        public async Task<Transaction> CreateProcurementTransaction(int numberOfLines, Partner partner = null, bool createNewProducts = true)
        {
            partner ??= (await AddPartners(1)).First();

            var products = createNewProducts
                ? await AddProducts(numberOfLines)
                : await _data.GetAllAsync<Product>();

            if (products.Count < numberOfLines)
                throw new ArgumentException($"Cannot create {numberOfLines} transaction lines, " +
                    $"because there are only {products.Count} products. " +
                    $"You can set '{createNewProducts}' to true to create the necessary products.", nameof(numberOfLines));

            int quantity = 1;
            var lines = products.Take(numberOfLines).Select(p => (p, quantity++)).ToList();

            var transaction = partner.ProcureFrom(lines);
            await _data.AddAsync(transaction);

            return transaction;
        }

        public async Task<Transaction> CreateSalesTransaction(Partner partner, IEnumerable<Product> productsToSell, float ratioOfStockToSell = 1f)
        {
            if (ratioOfStockToSell <= 0 || ratioOfStockToSell > 1)
                throw new ArgumentException("Ratio must be greater than 0 and less than 1.", nameof(ratioOfStockToSell));

            if (productsToSell.Any(p => p.NumberInStock == 0))
                throw new ArgumentException("All products must have a stock to be able to sell them.", nameof(ratioOfStockToSell));

            var lines = productsToSell.Select(p => (p, (int)Math.Ceiling(p.NumberInStock * ratioOfStockToSell))).ToList();

            var transaction = partner.SellTo(lines);
            await _data.AddAsync(transaction);

            return transaction;
        }

        /// <summary>
        /// Creates a sales transaction by automatically creating first a backing procurement transaction
        /// which in turn creates the necessary products for the transaction.
        /// </summary>
        /// <param name="partner">The partner to create a transaction with. If not provided, a new partner will be created.</param>
        /// <returns>Returns the sales transaction created. The result in the database will be
        /// the added products, the procurement transaction, and the sales transaction.</returns>
        public async Task<Transaction> CreateSalesTransaction(int numberOfLines, Partner partner = null)
        {
            partner ??= (await AddPartners(1)).First();

            var procurement = await CreateProcurementTransaction(numberOfLines, partner);

            var salesTransaction = partner.SellTo(procurement.TransactionLines.Select(p => (p.Product, p.Quantity)));
            await _data.AddAsync(salesTransaction);

            return salesTransaction;
        }
    }
}