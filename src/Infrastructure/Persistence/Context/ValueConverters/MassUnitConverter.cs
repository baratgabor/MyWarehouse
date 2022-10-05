using MyWarehouse.Domain.Common.ValueObjects.Mass;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace MyWarehouse.Infrastructure.Persistence.Context;

public class MassUnitConverter : ValueConverter<MassUnit, string>
{
    public MassUnitConverter()
        : base(
            massUnit => massUnit.Symbol,
            unitSymbol => MassUnit.FromSymbol(unitSymbol))
    {}
}
