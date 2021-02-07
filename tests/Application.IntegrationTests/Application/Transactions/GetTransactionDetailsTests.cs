using FluentAssertions;
using MyWarehouse.Application.Common.Exceptions;
using MyWarehouse.Application.Transactions.GetTransactionDetails;
using NUnit.Framework;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MyWarehouse.Core.IntegrationTests.Application.Transactions
{
    public class GetTransactionDetailsTests : TestBase
    {
        [Test]
        public void WhenIdIsInvalid_ShouldThrow_EntityNotFoundException()
        {
            var command = new GetTransactionDetailsQuery() { Id = 1234567 };

            FluentActions.Invoking(() => TestFramework.SendAsync(command))
                .Should().ThrowExactly<EntityNotFoundException>();
        }

        [Test]
        public async Task WhenIdValid_ShouldReturnTransactionDetails()
        {
            var existing = await TestFramework.DataFactory.CreateProcurementTransaction(4);

            var result = await TestFramework.SendAsync(new GetTransactionDetailsQuery() { Id = existing.Id });

            result.Should().NotBeNull();
            result.CreatedAt.Should().BeCloseTo(DateTime.Now, 10000);
            result.PartnerId.Should().Be(existing.PartnerId);
            result.PartnerName.Should().Be(existing.Partner.Name);
            result.PartnerAddress.Should().Be(existing.Partner.Address.ToString());
            result.TotalAmount.Should().Be(existing.Total.Amount);
            result.TotalCurrencyCode.Should().Be(existing.Total.Currency.Code);
            result.TransactionType.Should().Be((int)existing.TransactionType);
            result.TransactionLines
                .Should().BeEquivalentTo(existing.TransactionLines
                .Select(ent => new { 
                    ent.ProductId,
                    ent.Quantity,
                    ProductName = ent.Product.Name,
                    UnitPrice = ent.UnitPrice.ToString(),
                    UnitPriceAmount = ent.UnitPrice.Amount,
                    UnitPriceCurrencyCode = ent.UnitPrice.Currency.Code }),
                o => o.ExcludingMissingMembers());
        }
    }
}
