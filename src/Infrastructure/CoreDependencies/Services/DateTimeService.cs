using MyWarehouse.Application.Dependencies.Services;
using System;

namespace MyWarehouse.Infrastructure.CoreDependencies.Services
{
    internal class DateTimeService : IDateTime
    {
        public DateTime Now => DateTime.Now;
    }
}
