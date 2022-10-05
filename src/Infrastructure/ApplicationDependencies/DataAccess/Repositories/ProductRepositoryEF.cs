using System.Data;
using MyWarehouse.Domain.Products;
using Microsoft.EntityFrameworkCore;
using MyWarehouse.Infrastructure.Persistence.Context;
using MyWarehouse.Application.Common.Dependencies.DataAccess.Repositories;
using MyWarehouse.Infrastructure.ApplicationDependencies.DataAccess.Repositories.Common;

namespace MyWarehouse.Infrastructure.ApplicationDependencies.DataAccess.Repositories;

internal class ProductRepositoryEF : RepositoryBaseEF<Product>, IProductRepository
{
    protected override IQueryable<Product> BaseQuery
        => _context.Products.Include(x => x.Mass);

    public ProductRepositoryEF(ApplicationDbContext context, IMapper mapper) : base(context, mapper)
    { }

    public Task<List<Product>> GetHeaviestProducts(int numberOfProducts)
        => BaseQuery
            .OrderByDescending(p => p.Mass)
            .Take(numberOfProducts)
            .ToListAsync();

    public Task<List<Product>> GetMostStockedProducts(int numberOfProducts)
        => BaseQuery
            .OrderByDescending(p => p.NumberInStock)
            .Take(numberOfProducts)
            .ToListAsync();

    public override void Remove(Product entityToDelete)
    {
        _context.Remove(entityToDelete);
    }

    public override void RemoveRange(IEnumerable<Product> entitiesToDelete)
    {
        foreach (var e in entitiesToDelete)
            Remove(e);
    }
}
