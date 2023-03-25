using FluentAssertions;
using MyWarehouse.Domain;
using MyWarehouse.Domain.Common.ValueObjects.Mass;
using MyWarehouse.Domain.Common.ValueObjects.Money;
using MyWarehouse.Domain.Exceptions;
using MyWarehouse.Domain.Partners;
using MyWarehouse.Domain.Products;
using MyWarehouse.Domain.Transactions;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MyWarehouse.Domain.UnitTests.Transactions
{
    public class TransactionTests
    {
        [Test]
        public void NewTransaction_AssignsCorrectValues()
        {
            var partner = TestHelper.NewValidPartner;
            var transaction = new Transaction(TransactionType.Procurement, partner);

            transaction.TransactionType.Should().Be(TransactionType.Procurement);
            transaction.Partner.Should().Be(partner);
            transaction.Total.Amount.Should().Be(0);
            transaction.TransactionLines.Should()
                .NotBeNull().And
                .BeEmpty();
        }

        [Test]
        public void AddTransactionLine_ShouldThrow_WhenProductIsNull()
        {
            var transaction = TestHelper.NewTransaction_EmptyProcurement;

            FluentActions.Invoking(() => transaction.AddTransactionLine(null, 5)).Should().
                ThrowExactly<ArgumentNullException>().WithMessage("*product*");
        }

        [Test]
        public void AddTransactionLine_ShouldThrow_WhenQuantityIsNotPositive()
        {
            var transaction = TestHelper.NewTransaction_EmptyProcurement;
            var product = TestHelper.NewValidProduct_NoStock;

            FluentActions.Invoking(() => transaction.AddTransactionLine(product, 0)).Should().
                ThrowExactly<ArgumentException>().WithMessage("*quantity*");
        }

        [Test]
        public void AddTransactionLine_CreatesNewLine()
        {
            var transaction = TestHelper.NewTransaction_EmptyProcurement;
            var product = TestHelper.NewValidProduct_NoStock;

            transaction.AddTransactionLine(product, 4);

            transaction.TransactionLines.Should().HaveCount(1)
                .And.Subject.First().Should().BeEquivalentTo(new { 
                    Quantity = 4,
                    Product = product,
                    UnitPrice = product.Price
                }, o => o.ExcludingMissingMembers());
        }

        [Test]
        public void AddTransactionLine_CalculatesCorrectTotal()
        {
            var transaction = TestHelper.NewTransaction_EmptyProcurement;
            var product = TestHelper.NewValidProduct_NoStock;

            transaction.AddTransactionLine(product, 4);
            transaction.AddTransactionLine(product, 3);
            transaction.AddTransactionLine(product, 1);

            transaction.Total.Amount.Should().Be(product.Price.Amount * (4 + 3 + 1));
            transaction.Total.Currency.Should().Be(product.Price.Currency);
        }

        [Test]
        public void AddTransactionLine_OnProcurement_IncreasesStock()
        {
            var transaction = TestHelper.NewTransaction_EmptyProcurement;
            var product = TestHelper.NewValidProduct_WithStock(3);

            transaction.AddTransactionLine(product, 14);

            product.NumberInStock.Should().Be(3 + 14);
        }

        [Test]
        public void AddTransactionLine_OnSales_ShouldThrow_WhenProductHasInsufficientStock()
        {
            var transaction = TestHelper.NewTransaction_EmptySales;
            var product = TestHelper.NewValidProduct_WithStock(3);

            FluentActions.Invoking(() => transaction.AddTransactionLine(product, 4)).Should().
                ThrowExactly<InsufficientStockException>();
        }

        [Test]
        public void AddTransactionLine_OnSales_DecreasesStock()
        {
            var transaction = TestHelper.NewTransaction_EmptySales;
            var product = TestHelper.NewValidProduct_WithStock(10);

            transaction.AddTransactionLine(product, 3);

            product.NumberInStock.Should().Be(10 - 3);
        }

        [Test]
        public void AddTransactionLine_OnSales_CalculatesCorrectTotal()
        {
            var transaction = TestHelper.NewTransaction_EmptyProcurement;
            var product = TestHelper.NewValidProduct_NoStock;

            transaction.AddTransactionLine(product, 4);
            transaction.AddTransactionLine(product, 3);
            transaction.AddTransactionLine(product, 1);

            transaction.Total.Amount.Should().Be(product.Price.Amount * (4 + 3 + 1));
            transaction.Total.Currency.Should().Be(product.Price.Currency);
        }
    }
}
