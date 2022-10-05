using FluentAssertions;
using MyWarehouse.Application.Common.Exceptions;
using MyWarehouse.Application.Products.GetProduct;
using NUnit.Framework;
using System.Linq;
using System.Threading.Tasks;

namespace MyWarehouse.Application.IntegrationTests.Products
{
    public class GetProductDetailsTests : TestBase
    {
        [Test]
        public void WhenIdIsInvalid_ShouldThrow_EntityNotFoundException()
        {
            var request = new GetProductDetailsQuery() { Id = 12345 };

            FluentActions.Invoking(() => TestFramework.SendAsync(request))
                .Should().ThrowExactlyAsync<EntityNotFoundException>();
        }

        [Test]
        public async Task WhenIdIsValid_ShouldReturnProductDetails()
        {
            var existingProduct = (await TestFramework.DataFactory.AddProducts(1)).First();
            var request = new GetProductDetailsQuery() { Id = existingProduct.Id };

            var result = await TestFramework.SendAsync(request);

            result.Should().NotBeNull();
            result.Id.Should().Be(existingProduct.Id);
            result.Name.Should().Be(existingProduct.Name);
            result.NumberInStock.Should().Be(existingProduct.NumberInStock);
            result.Description.Should().Be(existingProduct.Description);
            result.MassValue.Should().Be(existingProduct.Mass.Value);
            result.MassUnitSymbol.Should().Be(existingProduct.Mass.Unit.Symbol);
            result.PriceAmount.Should().Be(existingProduct.Price.Amount);
            result.PriceCurrencyCode.Should().Be(existingProduct.Price.Currency.Code);
        }
    }
}
