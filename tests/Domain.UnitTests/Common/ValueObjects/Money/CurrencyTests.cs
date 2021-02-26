using FluentAssertions;
using MyWarehouse.Domain.Common.ValueObjects.Money;
using NUnit.Framework;
using System;

namespace MyWarehouse.Domain.UnitTests.Common.ValueObjects.MoneyTests
{
    public class CurrencyTests
    {
        [TestCase("USD")]
        [TestCase("EUR")]
        [TestCase("JPY")]
        [TestCase("GBP")]
        [TestCase("CAD")]
        [TestCase("AUD")]
        [TestCase("CHF")]
        [TestCase("NZD")]
        [TestCase("RUB")]
        [TestCase("HUF")]

        public void FromSymbol_WithValidSymbol_ReturnsCorrectUnit(string code)
        {
            var result = Currency.FromCode(code);

            result.Code.Should().Be(code);
            result.Symbol.Should().NotBeEmpty();
        }

        [Test]
        public void FromSymbol_WithInCorrectSymbol_ThrowsArgumentException()
        {
            FluentActions.Invoking(() => Currency.FromCode("abcabc"))
                .Should().ThrowExactly<ArgumentException>();
        }
    }
}
