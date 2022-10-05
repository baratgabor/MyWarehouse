using FluentAssertions;
using MyWarehouse.Application.Common.Exceptions;
using MyWarehouse.Application.Transactions.CreateTransaction;
using MyWarehouse.Domain.Partners;
using MyWarehouse.Domain.Products;
using MyWarehouse.Domain.Transactions;
using NUnit.Framework;
using System.Linq;
using System.Threading.Tasks;


namespace MyWarehouse.Application.IntegrationTests.Transactions
{
    public class CreateTransactionTests : TestBase
    {
        [Test]
        public void WhenDtoIsBlank_ShouldThrow_InputValidationException()
        {
            var command = new CreateTransactionCommand();

            FluentActions.Invoking(() => TestFramework.SendAsync(command))
                .Should().ThrowExactlyAsync<InputValidationException>();
        }

        [Test]
        public async Task ProcurementRequest_ShouldCreateTransaction()
        {
            var partner = (await TestFramework.DataFactory.AddPartners(1)).First();
            var products = await TestFramework.DataFactory.AddProducts(3);
            var command = new CreateTransactionCommand()
            {
                PartnerId = partner.Id,
                TransactionType = Domain.TransactionType.Procurement,
                TransactionLines = products.Select(p => new CreateTransactionCommand.TransactionLine() {
                    ProductId = p.Id,
                    ProductQuantity = 5}).ToArray()
            };

            // Act.
            var result = await TestFramework.SendAsync(command);

            var transaction = await TestFramework.Data.FindAsync<Transaction>(result, x => x.TransactionLines, x => x.Partner);
            transaction.Should().NotBeNull();
            transaction.Partner.Id.Should().Be(partner.Id);
            transaction.Total.Amount.Should().Be(transaction.TransactionLines.Sum(x => x.UnitPrice.Amount * x.Quantity));
            transaction.TransactionLines.Should().HaveSameCount(products)
                .And.Subject.Select(x => new { x.ProductId, ProductQuantity = x.Quantity })
                .Should().BeEquivalentTo(command.TransactionLines, 
                    o => o.ComparingByMembers<CreateTransactionCommand.TransactionLine>());

            var newProducts = await TestFramework.Data.GetAllAsync<Product>();
            newProducts.Select(x => new { id = x.Id, qty = x.NumberInStock })
                .Should().BeEquivalentTo(command.TransactionLines
                .Select(x => new { id = x.ProductId, qty = x.ProductQuantity }));
        }

        [Test]
        public async Task WhenInvalidProductIdPresent_ShouldThrowException()
        {
            var partner = (await TestFramework.DataFactory.AddPartners(1)).First();
            var products = await TestFramework.DataFactory.AddProducts(3);
            var command = new CreateTransactionCommand()
            {
                PartnerId = partner.Id,
                TransactionType = Domain.TransactionType.Procurement,
                TransactionLines = products.Select(p => new CreateTransactionCommand.TransactionLine()
                {
                    ProductId = p.Id,
                    ProductQuantity = 5
                }).Concat(new CreateTransactionCommand.TransactionLine[] { new()
                {
                    ProductId = 62614, // Another line added with invalid product Id.
                    ProductQuantity = 2
                }
                })
                .ToArray()
            };

            (await FluentActions.Invoking(() => TestFramework.SendAsync(command))
                .Should().ThrowExactlyAsync<InputValidationException>())
                    .Which.Errors.Should().ContainKey("ProductId")
                    .And.Subject["ProductId"].Should().ContainMatch("*62614*");
        }

        [Test]
        public async Task SalesRequest_ShouldCreateTransaction()
        {
            CreateTransactionCommand.TransactionLine[] lines;
            Partner partner;
            
            {   // Create procurement transaction to prepare product stock for sales.
                var t = await TestFramework.DataFactory.CreateProcurementTransaction(3);
                lines = t.TransactionLines.Select(x => new CreateTransactionCommand.TransactionLine()
                {
                    ProductId = x.Product.Id,
                    ProductQuantity = x.Quantity
                }).ToArray();
                partner = t.Partner;
            }
            
            var command = new CreateTransactionCommand()
            {
                TransactionType = Domain.TransactionType.Sales,
                PartnerId = partner.Id,
                TransactionLines = lines
            };

            // Act.
            var result = await TestFramework.SendAsync(command);

            var transaction = await TestFramework.Data.FindAsync<Transaction>(result, x => x.TransactionLines, x => x.Partner);
            transaction.Should().NotBeNull();
            transaction.Partner.Id.Should().Be(partner.Id);
            transaction.Total.Amount.Should().Be(transaction.TransactionLines.Sum(x => x.UnitPrice.Amount * x.Quantity));
            transaction.TransactionLines.Should().HaveSameCount(lines)
                .And.Subject.Select(x => new { x.ProductId, ProductQuantity = x.Quantity })
                .Should().BeEquivalentTo(command.TransactionLines,
                    o => o.ComparingByMembers<CreateTransactionCommand.TransactionLine>());

            var products = await TestFramework.Data.GetAllAsync<Product>();
            products.Should().Match(x => x.All(p => p.NumberInStock == 0)); // Because we sold the exact amount we procured.
        }

        [Test]
        public async Task SalesRequest_WithoutStock_ShouldThrowException()
        {
            var partner = (await TestFramework.DataFactory.AddPartners(1)).First();
            var products = await TestFramework.DataFactory.AddProducts(2);

            var command = new CreateTransactionCommand()
            {
                TransactionType = Domain.TransactionType.Sales,
                PartnerId = partner.Id,
                TransactionLines = products.Select(p => new CreateTransactionCommand.TransactionLine()
                {
                    ProductId = p.Id,
                    ProductQuantity = 5
                }).ToArray()
            };

            (await FluentActions.Invoking(() => TestFramework.SendAsync(command))
                .Should().ThrowExactlyAsync<InputValidationException>())
                    .And.Errors.SelectMany(e => e.Value).Should().ContainMatch("*stock*");
        }
    }
}
