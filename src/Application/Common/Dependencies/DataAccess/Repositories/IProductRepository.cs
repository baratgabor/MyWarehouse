using MyWarehouse.Application.Common.Dependencies.DataAccess.Repositories.Common;
using MyWarehouse.Domain.Products;

namespace MyWarehouse.Application.Common.Dependencies.DataAccess.Repositories;

public interface IProductRepository : IRepository<Product>
{
    Task<List<Product>> GetHeaviestProducts(int numberOfProducts);
    Task<List<Product>> GetMostStockedProducts(int numberOfProducts);
}
