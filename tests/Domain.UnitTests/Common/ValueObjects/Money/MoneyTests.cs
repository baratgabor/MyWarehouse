using MyWarehouse.Domain.Common.ValueObjects.Money;
using NUnit.Framework;
using System;

namespace MyWarehouse.Domain.UnitTests.Common.ValueObjects.MoneyTests
{
    public class MoneyTests
    {
        private Random _rnd = new Random();

        [Test]
        public void Instantiation_WithNegativeValue_ShouldThrow()
        {
            Assert.Throws<ArgumentException>(() => new Money(-100, Currency.EUR));
        }

        [Test]
        public void Add_WithMixedCurrencies_ShouldThrow()
        {
            Assert.Throws<InvalidOperationException>(() =>
                _ = new Money(100, Currency.EUR) + new Money(100, Currency.USD));
        }

        [Test, Repeat(10)]
        public void Add_WithSameCurrencies_ShouldReturnExpectedValue()
        {
            var m1 = new Money(_rnd.Next() / 1000, Currency.USD);
            var m2 = new Money(_rnd.Next() / 1000, Currency.USD);

            var result = m1 + m2;

            Assert.AreEqual(m1.Amount + m2.Amount, result.Amount);
            Assert.AreEqual(m1.Currency, result.Currency);
        }

        [Test]
        public void Multiply_WithMixedCurrencies_ShouldThrow()
        {
            Assert.Throws<InvalidOperationException>(() =>
                _ = new Money(100, Currency.EUR) * new Money(100, Currency.USD));
        }

        [Test, Repeat(10)]
        public void Multiply_WithSameCurrencies_ShouldReturnExpectedValue()
        {
            var m1 = new Money(_rnd.Next() / 1000, Currency.USD);
            var m2 = new Money(_rnd.Next() / 1000, Currency.USD);

            var result = m1 * m2;

            Assert.AreEqual(m1.Amount * m2.Amount, result.Amount);
            Assert.AreEqual(m1.Currency, result.Currency);
        }

        [Test]
        public void Subtract_WithMixedCurrencies_ShouldThrow()
        {
            Assert.Throws<InvalidOperationException>(() =>
                _ = new Money(100, Currency.EUR) - new Money(100, Currency.USD));
        }

        [Test, Repeat(10)]
        public void Subtract_WithSameCurrencies_ShouldReturnExpectedValue()
        {
            var m1 = new Money(_rnd.Next(1, 100000), Currency.USD);
            var m2 = new Money(_rnd.Next(1, (int)m1.Amount), Currency.USD);

            var result = m1 - m2;

            Assert.AreEqual(m1.Amount - m2.Amount, result.Amount);
            Assert.AreEqual(m1.Currency, result.Currency);
        }
    }
}
