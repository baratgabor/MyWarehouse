using FluentAssertions;
using MyWarehouse.Application.Common.Exceptions;
using MyWarehouse.Application.Partners.UpdatePartner;
using MyWarehouse.Domain.Products;
using NUnit.Framework;
using System.Linq;
using System.Threading.Tasks;

namespace MyWarehouse.Application.IntegrationTests.Products
{
    public class UpdateProductTests : TestBase
    {
        [Test]
        public void WhenDtoIsBlank_ShouldThrow_InputValidationException()
        {
            FluentActions.Invoking(() => TestFramework.SendAsync(new UpdateProductCommand()))
                .Should().ThrowExactly<InputValidationException>();
        }

        [Test]
        public async Task WhenDtoIsValid_ShouldUpdateProduct()
        {
            var product = (await TestFramework.DataFactory.AddProducts(1)).First();
            var command = new UpdateProductCommand()
            {
                Id = product.Id,
                Name = "NewProductName",
                Description = "NewProductDescription",
                MassValue = 1111.1f,
                PriceAmount = 9999.99m,
            };
            
            await TestFramework.SendAsync(command);

            var updatedProduct = await TestFramework.Data.FindAsync<Product>(command.Id);
            updatedProduct.Should().NotBeNull();
            updatedProduct.Id.Should().Be(command.Id);
            updatedProduct.Name.Should().Be(command.Name);
            updatedProduct.Description.Should().Be(command.Description);
            updatedProduct.Mass.Value.Should().Be(command.MassValue);
            updatedProduct.Price.Amount.Should().Be(command.PriceAmount);
        }
    }
}
