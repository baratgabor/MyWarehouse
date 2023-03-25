using MyWarehouse.Domain.Common.ValueObjects.Mass;
using MyWarehouse.Domain.Common.ValueObjects.Money;
using MyWarehouse.Domain.Products;

namespace MyWarehouse.TestData.Samples;

/// <summary>
/// Generates goofy sci-fi themed products.
/// </summary>
internal static class SampleProducts
{
    private static readonly string[] _namePrefixes = { "Bio-electric", "Neural", "Isograted", "Isolinear", "Microdyne", "Phylum", "Matterstream", "Transwarp", "Plasma", "Holographic", "Temporal", "Antimatter", "Dark matter", "Quantum", "Hydrogen", "Biogel", "RNA", "Void" };
    private static readonly string[] _nameSuffixes = { "Cable", "Diode", "Transponder", "Inducer", "Coupler", "Relay", "Coil", "Scanner", "Vacillator", "Inhibitor", "Oscillator", "Generator", "Inducer", "Reductor", "Splicer", "Transmuter", "Orchestrator", "Analyzer", "Doodad" };
    private static int MaximumNumber => _namePrefixes.Length * _nameSuffixes.Length;

    private static readonly string[] _descriptionPrefixes = { "Manages the", "Controls the", "Enhances the", "Distributes the", "Transforms the", "Acts as a governor in the", "Experimental version. Ensures the", "Plays a stabilizing role pertaining to the", "Quantifiably transposes the" };
    private static readonly string[] _descriptionJoiners = { "interaction of", "flow of", "connections between", "transfusions of", "intricate interconnections within", "seaming reagents of", "surrogate gyroconnections over" };
    private static readonly string[] _descriptionSuffixes = { "advanced micro circuits", "superconductive neural agents", "parallel quantum particles", "manifold dermal quantifiers", "charged stellar remains", "vorachodric micro-fitted interval dischargers", "exometric and telokinetic nano-engines", "tubular and oxogenic micoplasmosis" };

    private static readonly Random _rnd = new();

    internal static List<Product> GenerateSampleProducts(int number)
    {
        if (number > MaximumNumber)
        {
            throw new ArgumentException($"Maximum {MaximumNumber} unique products can be generated.", nameof(number));
        }

        var uniqueNames = new HashSet<string>(number);
        while (uniqueNames.Count < number)
        {
            uniqueNames.Add(GetName()); // HashSet filters out non-unique.
        }

        return uniqueNames.Select(name => new Product(
            name: name,
            description: GetDescription(),
            price: new Money(_rnd.Next(10, 999) + 0.99M, Currency.Default),
            mass: new Mass(_rnd.Next(10, 200) * 0.1f, MassUnit.Kilogram)
        )).ToList();

        string GetName()
             => $"{GetRandom(_namePrefixes)} {GetRandom(_nameSuffixes)}";

        string GetDescription()
             => $"{GetRandom(_descriptionPrefixes)} {GetRandom(_descriptionJoiners)} {GetRandom(_descriptionSuffixes)}";

        string GetRandom(string[] arr)
            => arr[_rnd.Next(arr.Length)];
    }
}
