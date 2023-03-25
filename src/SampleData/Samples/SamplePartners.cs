using MyWarehouse.Domain.Partners;

namespace MyWarehouse.TestData.Samples;

/// <summary>
/// Returns some sci-fi themed partners.
/// </summary>
internal static class SamplePartners
{
    internal static List<Partner> GetSamplePartners()
    {
        return new List<Partner>()
        {
            new Partner(
                name: "Weyland-Yutani Corp",
                address: new Address("1 Weyland Way", "Tokyo", "Japan", "100-6007")
            ),
            new Partner(
                name: "Microsoft",
                address: new Address("One Microsoft Way", "Redmond", "USA", "WA 98052")
            ),
            new Partner(
                name: "TranStar Corporation",
                address: new Address("2 Arboretum Bay", "Talos I", "China", "TAB2")
            ),
            new Partner(
                name: "Cyberdyne Systems",
                address: new Address("18144 El Camino Real", "Sunnyvale", "USA", "CA 93960")
            ),
            new Partner(
                name: "Darkbook",
                address: new Address("1 Humans Are Data Way", "Serverside", "USA", "CA 95960")
            ),
            new Partner(
                name: "Tesla Cryogenics",
                address: new Address("126 Frozen Way", "Palo Alto", "USA", "CA 94304")
            ),
            new Partner(
                name: "Blue Sun Corporation",
                address: new Address("1 Blue Sun HQ", "New Cardiff", "Australia", "2020")
            ),
            new Partner(
                name: "Spacely Space Sprockets",
                address: new Address("94 Propellant Boulevard", "Core York", "USA", "SPRC-D92F4")
            ),
            new Partner(
                name: "Yoyodyne Propulsion Sys.",
                address: new Address("R. do Rossio 1", "Madrid", "Spain", "28320")
            ),
            new Partner(
                name: "Virtucon",
                address: new Address("Mint Park Woodway Lane", "Leicestershire", "UK", "LE17 5FB")
            ),
            new Partner(
                name: "Tyrell Corp.",
                address: new Address("8 Nexus Center", "Los Angeles", "USA", "NE/444")
            ),
        };
    }
}
