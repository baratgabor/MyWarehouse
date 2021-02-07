using MediatR;
using Microsoft.Extensions.DependencyInjection;
using MyWarehouse.Domain.Common;
using MyWarehouse.Domain.Partners;
using MyWarehouse.Domain.Products;
using MyWarehouse.Domain.Transactions;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace MyWarehouse.Core.IntegrationTests
{
    [SetUpFixture]
    public class TestFramework
    {
        private static TestHost _host;
        private static TestData _data;
        private static TestDataFactory _dataFactory;

        [OneTimeSetUp]
        public void SetUpEnvironment()
        {
            _host = TestHost.Create();
            _data = new TestData(_host);
            _dataFactory = new TestDataFactory(_data);
        }

        /// <summary>
        /// Executes the provided request in a newly instantiated scope,
        /// to isolate it from the rest of the test. Be aware that test DbContext
        /// will be renewed during this call, to ensure that test code won't pick up
        /// cached entities, because that could falsify test results.
        /// <see cref="https://stackoverflow.com/a/17051820/2906385"/>
        /// </summary>
        // Note that serving everyhing with AsNoTracking() to the tests is
        // not a good alternative to context renewal, because it would lead to DB PK collisions
        // when tests try to insert entities with untracked navigation properties set on them.
        public static async Task<TResponse> SendAsync<TResponse>(IRequest<TResponse> request)
        {
            _data.DisposeScope();

            using var scope = _host.ScopeFactory.CreateScope();
            var mediator = scope.ServiceProvider.GetService<IMediator>();
            var result = await mediator.Send(request);

            _data.CreateScope();

            return result;
        }

        // TODO: Find a better solution. Especially because this hides the method documentation.
        // Methods are exposed this way because I like structured access, but if I used properties, FluentAssertions
        // would have polluted the intellisense list with its extensions methods, which I find extremely annoying.
        public static class Data
        {
            public static Task<int> AddAsync<TEntity>(TEntity entity) where TEntity : IEntity
                => _data.AddAsync(entity);

            public static Task AddRangeAsync<TEntity>(params TEntity[] entities) where TEntity : class, IEntity
                => _data.AddRangeAsync(entities);

            public static Task<List<TEntity>> GetAllAsync<TEntity>(params Expression<Func<TEntity, object>>[] includes) where TEntity : class, IEntity
                => _data.GetAllAsync(includes);

            public static Task<TEntity> FindAsync<TEntity>(int id, params Expression<Func<TEntity, object>>[] includes) where TEntity : class, IEntity
                => _data.FindAsync(id, includes);

            public static Task ResetDatabase()
                => _data.ResetDatabase();
        }

        public static class Context
        {
            public static void CreateScope() => _data.CreateScope();
            public static void DisposeScope() => _data.DisposeScope();
        }

        public static class DataFactory
        {
            public static Task<List<Partner>> AddPartners(int howMany, Action<(int index, Partner partner)> partnerOverrides = null) 
                => _dataFactory.AddPartners(howMany, partnerOverrides);

            public static Task<List<Product>> AddProducts(int howMany, Action<(int index, Product product)> productOverrides = null)
                => _dataFactory.AddProducts(howMany, productOverrides);

            public static Task<Transaction> CreateProcurementTransaction(int howManyLines, Partner partner = null, bool createNewProducts = true)
                => _dataFactory.CreateProcurementTransaction(howManyLines, partner, createNewProducts);

            public static Task<Transaction> CreateSalesTransaction(int howManyLines, Partner partner = null)
                => _dataFactory.CreateSalesTransaction(howManyLines, partner);

            public static Task<Transaction> CreateSalesTransaction(Partner partner, IEnumerable<Product> productsToSell, float ratioOfStockToSell = 1)
                => _dataFactory.CreateSalesTransaction(partner, productsToSell, ratioOfStockToSell);
        }
    }
}