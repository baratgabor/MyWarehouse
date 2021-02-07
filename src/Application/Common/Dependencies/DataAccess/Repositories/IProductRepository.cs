using MyWarehouse.Application.Common.Dependencies.DataAccess.Repositories.Common;
using MyWarehouse.Domain.Common.ValueObjects.Mass;
using MyWarehouse.Domain.Common.ValueObjects.Money;
using MyWarehouse.Domain.Products;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyWarehouse.Application.Common.Dependencies.DataAccess.Repositories
{
    public interface IProductRepository : IRepository<Product>
    {
        Task<List<Product>> GetHeaviestProducts(int numberOfProducts);
        Task<List<Product>> GetMostStockedProducts(int numberOfProducts);
    }
}