using MyWarehouse.Domain.Common.ValueObjects.Money;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace MyWarehouse.Infrastructure.Persistence.Context;

public class CurrencyConverter : ValueConverter<Currency, string>
{
    public CurrencyConverter()
        : base(
            currency => currency.Code,
            currencyCode => Currency.FromCode(currencyCode))
    {}
}
