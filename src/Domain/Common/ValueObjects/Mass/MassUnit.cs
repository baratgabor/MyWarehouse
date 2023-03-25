namespace MyWarehouse.Domain.Common.ValueObjects.Mass;

public record MassUnit
{
    public string Name { get; init; }
    public string Symbol { get; init; }
    public float ConversionRateToGram { get; init; }

    private MassUnit(string name, string symbol, float conversionRateToGram)
    {
        (Name, Symbol, ConversionRateToGram) = (name, symbol, conversionRateToGram);
    }

    public static readonly MassUnit Tonne       = new("tonne", "t", 1000000f);
    public static readonly MassUnit Kilogram    = new("kilogram", "kg", 1000f);
    public static readonly MassUnit Gram        = new("gram", "g", 1f);
    public static readonly MassUnit Pound       = new("pound", "lb", 453.59237f);

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
