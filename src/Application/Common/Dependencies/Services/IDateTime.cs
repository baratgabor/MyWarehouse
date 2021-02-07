using System;

namespace MyWarehouse.Application.Dependencies.Services
{
    public interface IDateTime
    {
        DateTime Now { get; }
    }
}
