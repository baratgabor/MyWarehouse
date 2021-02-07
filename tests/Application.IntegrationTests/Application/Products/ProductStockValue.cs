using FluentAssertions;
using MyWarehouse.Application.Products.ProductStockValue;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyWarehouse.Core.IntegrationTests.Application.Products
{
    class ProductStockValue : TestBase
    {
        [Test]
        public async Task ShouldReturnStockCount()
        {
            var products = await TestFramework.DataFactory.AddProducts(25);

            var result = await TestFramework.SendAsync(new ProductStockValueQuery());

            result.Should().NotBeNull();
            result.Amount.Should().Be(products
                .Sum(x => x.Price.Amount * x.NumberInStock));
            result.CurrencyCode.Should().Be(products.GroupBy(x => 
                x.Price.Currency.Code).Single().Key);
        }
    }
}

