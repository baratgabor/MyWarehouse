using FluentAssertions;
using MyWarehouse.Application.Transactions.GetTransactionsList;
using MyWarehouse.Domain;
using MyWarehouse.Domain.Products;
using NUnit.Framework;
using System.Linq;
using System.Threading.Tasks;

namespace MyWarehouse.Core.IntegrationTests.Application.Transactions
{
    public class GetTransactionsListTests : TestBase
    {
        [Test]
        public async Task WhenEmpty_ShouldReturnEmptyList()
        {
            var list = await TestFramework.SendAsync(new GetTransactionListQuery());

            list.Should().NotBeNull();
            list.RowCount.Should().Be(0);
            list.Results.Should().BeEmpty();
        }

        [Test]
        public async Task WhenAllTransactionsRequested_ShouldReturnPagedTransactions()
        {
            var numberOfTransactions = 25;
            var partner = (await TestFramework.DataFactory.AddPartners(1)).First();
            for (int i = 0; i < numberOfTransactions; i++)
            {
                await TestFramework.DataFactory.CreateProcurementTransaction(1, partner);
            }
            
            var list = await TestFramework.SendAsync(new GetTransactionListQuery());

            list.Should().NotBeNull();
            list.RowCount.Should().Be(numberOfTransactions);
            list.PageIndex.Should().Be(1);
            list.Results.Should().HaveCount(list.PageSize)
                .And.OnlyHaveUniqueItems();
        }

        [Test]
        public async Task WhenProcurementsRequested_ShouldReturnOnlyProcurements()
        {
            var numberOfProcurements = 6;
            var partner = (await TestFramework.DataFactory.AddPartners(1)).First();
            for (int i = 0; i < numberOfProcurements; i++)
            {
                await TestFramework.DataFactory.CreateProcurementTransaction(1, partner,
                    createNewProducts: true);
            }
            var products = await TestFramework.Data.GetAllAsync<Product>();
            for (int i = 0; i < 3; i++)
            {
                await TestFramework.DataFactory.CreateSalesTransaction(partner, products.Skip(i).Take(1));
            }

            var list = await TestFramework.SendAsync(new GetTransactionListQuery() {
                Type = TransactionType.Procurement });

            list.Should().NotBeNull();
            list.RowCount.Should().Be(numberOfProcurements);
            list.Results.Should().HaveCount(numberOfProcurements)
                .And.OnlyHaveUniqueItems()
                .And.OnlyContain(x => x.TransactionType == (int)TransactionType.Procurement);
        }

        [Test]
        public async Task WhenSalesRequested_ShouldReturnOnlySales()
        {
            var partner = (await TestFramework.DataFactory.AddPartners(1)).First();
            for (int i = 0; i < 6; i++)
            {
                await TestFramework.DataFactory.CreateProcurementTransaction(1, partner,
                    createNewProducts: true);
            }
            var numberOfSales = 3;
            var products = await TestFramework.Data.GetAllAsync<Product>();
            for (int i = 0; i < numberOfSales; i++)
            {
                await TestFramework.DataFactory.CreateSalesTransaction(partner, products.Skip(i).Take(1));
            }

            var list = await TestFramework.SendAsync(new GetTransactionListQuery()
            {
                Type = TransactionType.Sales
            });

            list.Should().NotBeNull();
            list.RowCount.Should().Be(numberOfSales);
            list.Results.Should().HaveCount(numberOfSales)
                .And.OnlyHaveUniqueItems()
                .And.OnlyContain(x => x.TransactionType == (int)TransactionType.Sales);
        }
    }
}
