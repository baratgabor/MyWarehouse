using MyWarehouse.Application.Dependencies.Services;

namespace MyWarehouse.Infrastructure.ApplicationDependencies.Services;

internal class DateTimeService : IDateTime
{
    public DateTime Now => DateTime.Now;
}
