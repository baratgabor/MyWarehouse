using System;

namespace MyWarehouse.Domain.Common.ValueObjects.Mass
{
    public record MassUnit
    {
        public string Name { get; init; }
        public string Symbol { get; init; }
        public float ConversionRateToGram { get; init; }

        private MassUnit()
        { }

        public static readonly MassUnit Tonne = new MassUnit() { Name = "tonne", Symbol = "t", ConversionRateToGram = 1000000f };
        public static readonly MassUnit Kilogram = new MassUnit() { Name = "kilogram", Symbol = "kg", ConversionRateToGram = 1000f };
        public static readonly MassUnit Gram = new MassUnit() { Name = "gram", Symbol = "g", ConversionRateToGram = 1f };
        public static readonly MassUnit Pound = new MassUnit() { Name = "pound", Symbol = "lb", ConversionRateToGram = 453.59237f };

        public static MassUnit FromSymbol(string unitSymbol)
            => unitSymbol.ToLower() switch
            {
                "t" => Tonne,
                "kg" => Kilogram,
                "g" => Gram,
                "lb" => Pound,
                _ => throw new ArgumentException($"Uknown {nameof(MassUnit)} value '{unitSymbol}'.", nameof(unitSymbol))
            };
    }
}
