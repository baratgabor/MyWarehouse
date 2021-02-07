using FluentAssertions;
using MyWarehouse.Domain.Partners;
using MyWarehouse.Domain.Products;
using MyWarehouse.Domain.Transactions;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MyWarehouse.Domain.UnitTests.Partners
{
    public class PartnerTests
    {
        [Test]
        public void NewPartner_Throws_WhenNameIsNullOrEmpty()
        {
            var partnerAddress = TestHelper.NewValidAddress;

            FluentActions.Invoking(() => new Partner(null, partnerAddress)).Should().
                ThrowExactly<ArgumentException>("null");

            FluentActions.Invoking(() => new Partner("", partnerAddress)).Should().
                ThrowExactly<ArgumentException>("empty");

            FluentActions.Invoking(() => new Partner("      ", partnerAddress)).Should().
                ThrowExactly<ArgumentException>("whitespace");
        }

        [Test]
        public void NewPartner_Throws_WhenNameIsEmpty()
        {
            FluentActions.Invoking(() => new Partner(null, TestHelper.NewValidAddress)).Should().
                ThrowExactly<ArgumentException>("null").WithMessage("*name*");

            FluentActions.Invoking(() => new Partner("", TestHelper.NewValidAddress)).Should().
                ThrowExactly<ArgumentException>("empty").WithMessage("*name*");

            FluentActions.Invoking(() => new Partner("   ", TestHelper.NewValidAddress)).Should().
                ThrowExactly<ArgumentException>("whitespace").WithMessage("*name*");
        }

        [Test]
        public void NewPartner_Throws_WhenAddressIsNull()
        {
            Action act = () => new Partner("John Test", null);

            FluentActions.Invoking(act).Should()
                .ThrowExactly<ArgumentNullException>();
        }

        [Test]
        public void NewPartner_HasAllPropertiesAssigned()
        {
            var partnerAddress = TestHelper.NewValidAddress;
            var partner = new Partner("John Test", partnerAddress);

            partner.Name.Should().Be("John Test");
            partner.Address.Should().NotBeNull()
                .And.BeSameAs(partnerAddress);
            partner.Transactions.Should()
                .NotBeNull().And
                .BeEmpty();
        }

        [Test]
        public void UpdateName_Throws_WhenNameIsNullOrEmpty()
        {
            var partner = TestHelper.NewValidPartner;

            FluentActions.Invoking(() => partner.UpdateName(null)).Should().
                ThrowExactly<ArgumentException>("null").WithMessage("*name*");

            FluentActions.Invoking(() => partner.UpdateName("")).Should().
                ThrowExactly<ArgumentException>("empty").WithMessage("*name*");

            FluentActions.Invoking(() => partner.UpdateName("      ")).Should().
                ThrowExactly<ArgumentException>("whitespace").WithMessage("*name*");
        }

        [Test]
        public void UpdateAddress_Throws_WhenAddressIsNull()
        {
            var partner = TestHelper.NewValidPartner;

            FluentActions.Invoking(() => partner.UpdateAddress(null)).Should().
                ThrowExactly<ArgumentNullException>();
        }

        [Test]
        public void SellTo_Throws_WhenLinesAreNullOrEmpty()
        {
            var partner = TestHelper.NewValidPartner;

            FluentActions.Invoking(() => partner.SellTo(null)).Should().
                ThrowExactly<ArgumentNullException>("null");

            FluentActions.Invoking(() => partner.SellTo(Enumerable.Empty<(Product, int)>())).Should().
                ThrowExactly<ArgumentException>("empty");
        }

        [Test]
        public void SellTo_Throws_WhenProductIsNull()
        {
            var partner = TestHelper.NewValidPartner;

            var lines = new List<(Product, int)>() {
                (null, 1)
            };

            FluentActions.Invoking(() => partner.SellTo(lines)).Should().
                ThrowExactly<ArgumentException>().WithMessage("*product*");
        }

        [Test]
        public void SellTo_Throws_WhenProductQuantityIsZeroOrLess()
        {
            var partner = TestHelper.NewValidPartner;

            var lines = new List<(Product, int)>() {
                (TestHelper.NewValidProduct_NoStock, 0)
            };

            FluentActions.Invoking(() => partner.SellTo(lines)).Should().
                ThrowExactly<ArgumentException>().WithMessage("*quantit*");
        }

        [Test]
        public void SellTo_Throws_WhenProductHasNoStock()
        {
            var partner = TestHelper.NewValidPartner;

            var lines = new List<(Product, int)>() {
                (TestHelper.NewValidProduct_NoStock, 2)
            };

            FluentActions.Invoking(() => partner.SellTo(lines)).Should().
                Throw<Exception>().WithMessage("*stock*");
        }

        [Test]
        public void SellTo_CreatesTransaction_WhenProductIsStocked()
        {
            var partner = TestHelper.NewValidPartner;
            var stockedProduct = TestHelper.NewValidProduct_WithStock(5);
            var lines = new List<(Product, int)>() {
                (stockedProduct, 2)
            };

            FluentActions.Invoking(() => partner.SellTo(lines)).Should()
                .NotThrow();

            partner.Transactions.Should().HaveCount(1)
                .And.Subject.First().TransactionType.Should().Be(TransactionType.Sales);
            partner.Transactions.First().TransactionLines.Should().HaveCount(1)
                .And.Subject.First().Should().Match<TransactionLine>(tl =>
                    tl.Product == stockedProduct && 
                    tl.Quantity == 2);

            stockedProduct.NumberInStock.Should().Be(5 - 2);
        }

        [Test]
        public void ProcureFrom_Throws_WhenLinesAreNullOrEmpty()
        {
            var partner = TestHelper.NewValidPartner;

            FluentActions.Invoking(() => partner.ProcureFrom(null)).Should().
                ThrowExactly<ArgumentNullException>("null");

            FluentActions.Invoking(() => partner.ProcureFrom(Enumerable.Empty<(Product, int)>())).Should().
                ThrowExactly<ArgumentException>("empty");
        }

        [Test]
        public void ProcureFrom_Throws_WhenProductIsNull()
        {
            var partner = TestHelper.NewValidPartner;

            var lines = new List<(Product, int)>() {
                (null, 1)
            };

            FluentActions.Invoking(() => partner.ProcureFrom(lines)).Should().
                ThrowExactly<ArgumentException>().WithMessage("*product*");
        }

        [Test]
        public void ProcureFrom_Throws_WhenProductQuantityIsZeroOrLess()
        {
            var partner = TestHelper.NewValidPartner;

            var lines = new List<(Product, int)>() {
                (TestHelper.NewValidProduct_NoStock, 0)
            };

            FluentActions.Invoking(() => partner.ProcureFrom(lines)).Should().
                ThrowExactly<ArgumentException>().WithMessage("*quantit*");
        }

        [Test]
        public void ProcureFrom_CreatesTransaction()
        {
            var partner = TestHelper.NewValidPartner;
            var product1 = TestHelper.NewValidProduct_NoStock;
            var product2 = TestHelper.NewValidProduct_NoStock;
            var lines = new List<(Product, int)>() {
                (product1, 2),
                (product2, 5)
            };

            FluentActions.Invoking(() => partner.ProcureFrom(lines)).Should()
                .NotThrow();

            partner.Transactions.Should().HaveCount(1)
                .And.Subject.First().TransactionType.Should().Be(TransactionType.Procurement);
            partner.Transactions.First().TransactionLines.Should().HaveCount(2)
                .And.BeEquivalentTo(new[] {
                    new { Product = product1, Quantity = 2 },
                    new { Product = product2, Quantity = 5 },
                }, o => o.ExcludingMissingMembers());

            product1.NumberInStock.Should().Be(2);
            product2.NumberInStock.Should().Be(5);
        }
    }
}