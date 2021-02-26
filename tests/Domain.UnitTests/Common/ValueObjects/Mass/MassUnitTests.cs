using FluentAssertions;
using MyWarehouse.Domain.Common.ValueObjects.Mass;
using NUnit.Framework;
using System;

namespace MyWarehouse.Domain.UnitTests.Common.ValueObjects.MassTests
{
    public class MassUnitTests
    {
        [TestCase("kg")]
        [TestCase("t")]
        [TestCase("lb")]
        [TestCase("g")]
        public void FromSymbol_WithValidSymbol_ReturnsCorrectUnit(string symbol)
        {
            var result = MassUnit.FromSymbol(symbol);

            result.Symbol.Should().Be(symbol);
        }

        [Test]
        public void FromSymbol_WithInCorrectSymbol_ThrowsArgumentException()
        {
            FluentActions.Invoking(() => MassUnit.FromSymbol("abcabc"))
                .Should().ThrowExactly<ArgumentException>();
        }
    }
}
