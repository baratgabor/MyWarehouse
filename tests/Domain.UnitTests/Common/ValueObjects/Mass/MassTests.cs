using System;
using MyWarehouse.Domain.Common.ValueObjects.Mass;
using NUnit.Framework;

namespace MyWarehouse.Domain.UnitTests.Common.ValueObjects.MassTests
{
    public class MassTests
    {
        private readonly Random _rnd = new Random();

        [Test]
        public void Instantiation_WithNegativeValue_ShouldThrow()
        {
            Assert.Throws<ArgumentException>(() => new Mass(-1, MassUnit.Kilogram));
        }

        [TestCase("g",  "g",  1)]
        [TestCase("g",  "kg", 0.001f)]
        [TestCase("g",  "t", 0.000001f)]
        [TestCase("g",  "lb", 0.00220462f)]
        [TestCase("kg", "kg", 1)]
        [TestCase("kg", "g",  1000)]
        [TestCase("kg", "t", 0.001f)]
        [TestCase("kg", "lb", 2.20462f)]
        [TestCase("lb", "lb", 1)]
        [TestCase("lb", "t", 0.000453592f)]
        [TestCase("lb", "kg", 0.45359237f)]
        [TestCase("lb", "g",  453.59237f)]
        [TestCase("t",  "t", 1)]
        [TestCase("t",  "g", 1000000)]
        [TestCase("t",  "kg", 1000)]
        [TestCase("t",  "lb", 2204.6226218f)]
        public void ConvertTo_ReturnsExpectedValue(string fromUnit, string toUnit, float expectedConversionRate)
        {
            var expectedPrecision = 0.02f;

            for (int i = 0; i < 10; i++) // RepeatAttribute doesn't work on TestCases.
            {
                var testValue = _rnd.Next(100);

                var result = new Mass(testValue, MassUnit.FromSymbol(fromUnit))
                    .ConvertTo(MassUnit.FromSymbol(toUnit));

                Assert.AreEqual(testValue * expectedConversionRate, result.Value, expectedPrecision);
                Assert.AreEqual(MassUnit.FromSymbol(toUnit), result.Unit);
            }
        }
    }
}
