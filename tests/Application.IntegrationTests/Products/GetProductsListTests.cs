using FluentAssertions;
using MyWarehouse.Application.Products.GetProducts;
using NUnit.Framework;
using System.Threading.Tasks;

namespace MyWarehouse.Application.IntegrationTests.Products
{
    public class GetProductsListTests : TestBase
    {
        [Test]
        public async Task WhenEmpty_ShouldReturnEmptyList()
        {
            var list = await TestFramework.SendAsync(new GetProductsListQuery());

            list.Should().NotBeNull();
            list.RowCount.Should().Be(0);
            list.Results.Should().BeEmpty();
        }

        [Test]
        public async Task WhenAllProductsRequested_ShouldReturnPagedProducts()
        {
            await TestFramework.DataFactory.AddProducts(31);

            // Act.
            var list = await TestFramework.SendAsync(new GetProductsListQuery()
            {
                Status = GetProductsListQuery.ProductStatus.Default
            });

            list.RowCount.Should().Be(31);
            list.PageIndex.Should().Be(1);
            list.Results.Should().NotBeNull()
                .And.HaveCount(list.PageSize)
                .And.NotContainNulls()
                .And.OnlyHaveUniqueItems();
        }

        [Test]
        public async Task WhenStockedProductsRequested_ShouldReturnOnlyStocked()
        {
            await TestFramework.DataFactory.AddProducts(31);
            await TestFramework.DataFactory.CreateProcurementTransaction(6,
                createNewProducts: false);

            // Act.
            var list = await TestFramework.SendAsync(new GetProductsListQuery()
            {
                Status = GetProductsListQuery.ProductStatus.Stocked
            });

            list.RowCount.Should().Be(6);
            list.Results.Should().NotBeNull()
                .And.HaveCount(6)
                .And.OnlyHaveUniqueItems()
                .And.OnlyContain(x => x.NumberInStock > 0);
        }
    }
}
