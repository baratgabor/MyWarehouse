using FluentAssertions;
using MyWarehouse.Application.Products.GetProductsSummary;
using NUnit.Framework;
using System.Linq;
using System.Threading.Tasks;

namespace MyWarehouse.Core.IntegrationTests.Application.Products
{
    public class ProductStockCount : TestBase
    {
        [Test]
        public async Task ShouldReturnStockCount()
        {
            var products = await TestFramework.DataFactory.AddProducts(21);

            var result = await TestFramework.SendAsync(new ProductStockCountQuery());

            result.Should().NotBeNull();
            result.ProductCount.Should().Be(products.Count);
            result.TotalStock.Should().Be(products.Sum(x => x.NumberInStock));
        }
    }
}
