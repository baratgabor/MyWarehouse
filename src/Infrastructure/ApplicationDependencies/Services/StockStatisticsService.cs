using Microsoft.EntityFrameworkCore;
using MyWarehouse.Application.Dependencies.Services;
using MyWarehouse.Domain.Common.ValueObjects.Mass;
using MyWarehouse.Domain.Common.ValueObjects.Money;
using MyWarehouse.Infrastructure.Persistence.Context;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MyWarehouse.Infrastructure.ApplicationDependencies.Services
{
    // It's rather questionable that this service is implemented here in the Infrastructure layer.
    // One could argue that it should be in Application, but, on the other hand,
    // it does depend on EF Core, and probably even specifically on SQL.
    class StockStatisticsService : IStockStatisticsService
    {
        private readonly ApplicationDbContext _dbContext;

        public StockStatisticsService(ApplicationDbContext dbContext)
            => _dbContext = dbContext;

        public async Task<(int ProductCount, int TotalStock)> GetProductStockCounts()
        {
            var res = await _dbContext.Products
                .GroupBy(x => 1)
                .Select(g => new {
                    productCount = g.Count(),
                    totalStock = g.Sum(p => p.NumberInStock)
                }).SingleAsync();

            return (res.productCount, res.totalStock);
        }

        public async Task<Mass> GetProductStockTotalMass(MassUnit unit)
        {
            var totalMassPerUnit = await _dbContext.Products
                .GroupBy(x => x.Mass.Unit, p => new
                {
                    p.Mass,
                    p.NumberInStock
                })
                .Select(g => new
                {
                    MassUnit = g.Key,
                    TotalMass = g.Sum(x => x.Mass.Value * x.NumberInStock)
                }).ToListAsync();

            var totalMass = totalMassPerUnit
                .Select(x => new Mass(x.TotalMass, x.MassUnit))
                .Sum(mass => mass.ConvertTo(unit).Value);

            return new Mass(totalMass, unit);
        }

        public async Task<Money> GetProductStockTotalValue()
        {
            var stockValuesPerCurrency = await _dbContext.Products
                .GroupBy(x => x.Price.Currency, p => new
                {
                    UnitPrice = p.Price.Amount,
                    Currency = p.Price.Currency,
                    NumberInStock = p.NumberInStock
                })
                .Select(g => new
                {
                    Currency = g.Key,
                    TotalValue = g.Sum(x => x.UnitPrice * x.NumberInStock)
                }).ToListAsync();

            if (stockValuesPerCurrency.Count > 1)
                throw new InvalidOperationException(
                    $"Operation cannot be completed, because not all product prices use the same currency. Distinct currencies detected: " +
                    $"{string.Join(", ", stockValuesPerCurrency.Select(x => x.Currency.Code))}.");

            var stockValue = stockValuesPerCurrency.First();
            return new Money(stockValue.TotalValue, Currency.FromCode(stockValue.Currency.Code));
        }
    }
}
