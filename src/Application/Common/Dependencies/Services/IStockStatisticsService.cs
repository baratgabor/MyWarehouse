using MyWarehouse.Domain.Common.ValueObjects.Mass;
using MyWarehouse.Domain.Common.ValueObjects.Money;
using System.Threading.Tasks;

namespace MyWarehouse.Application.Dependencies.Services
{
    public interface IStockStatisticsService
    {
        /// <summary>
        /// Returns the aggregate mass of all stocked products.
        /// </summary>
        Task<Mass> GetProductStockTotalMass(MassUnit unit);

        /// <summary>
        /// Returns the aggregate value of all stocked products.
        /// </summary>
        Task<Money> GetProductStockTotalValue();

        /// <summary>
        /// Returns the number of individual products available, and the total number of stocked products.
        /// </summary>
        Task<(int ProductCount, int TotalStock)> GetProductStockCounts();
    }
}