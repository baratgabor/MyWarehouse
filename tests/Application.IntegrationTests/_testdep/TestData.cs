using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MyWarehouse.Domain.Common;
using MyWarehouse.Infrastructure.Identity.Model;
using MyWarehouse.Infrastructure.Persistence.Context;
using Respawn;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace MyWarehouse.Application.IntegrationTests
{
    public class TestData
    {
        private readonly TestHost _host;
        private readonly Func<Task> _databaseReset;
        private ApplicationDbContext _dbContext;
        private IServiceScope _scope;

        public TestData(TestHost host)
        {
            _host = host;
            _databaseReset = () => {
                return new Checkpoint()
                {
                    TablesToIgnore = new[] { "__EFMigrationsHistory" }
                }
                .Reset(_host.ConnectionString);
            };

            {   // Ensure database.
                using var scope = _host.ScopeFactory.CreateScope();
                using var context = scope.ServiceProvider.GetService<ApplicationDbContext>();

                context.Database.Migrate();
            }
        }

        public void CreateScope()
        {
            if (_dbContext != null) _dbContext.Dispose();
            if (_scope != null) _scope.Dispose();

            _scope = _host.ScopeFactory.CreateScope();
            _dbContext = _scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        }

        public void DisposeScope()
        {
            _dbContext?.Dispose();
            _dbContext = null;
            _scope?.Dispose();
            _scope = null;
        }

        public async Task<int> AddAsync<TEntity>(TEntity entity) where TEntity : IEntity
        {
            _dbContext.Add(entity);
            await _dbContext.SaveChangesAsync();

            return entity.Id;
        }

        public async Task AddRangeAsync<TEntity>(params TEntity[] entities) where TEntity : class, IEntity
        {
            _dbContext.Set<TEntity>().AddRange(entities);
            await _dbContext.SaveChangesAsync();
        }

        public Task<T> FindAsync<T>(int id, params Expression<Func<T, object>>[] includes)
            where T : class, IEntity
        {
            var query = _dbContext.Set<T>().AsQueryable();
            foreach (var i in includes)
                query = query.Include(i);

            return query.SingleOrDefaultAsync(x => x.Id == id);
        }

        public Task<List<T>> GetAllAsync<T>(params Expression<Func<T, object>>[] includes)
            where T : class, IEntity
        {
            var query = _dbContext.Set<T>().AsQueryable();
            foreach (var i in includes)
                query = query.Include(i);

            return query.ToListAsync();
        }

        public async Task ResetDatabase()
            => await _databaseReset.Invoke();

        public async Task<string> AddUserAsync(string userName, string password)
        {
            var user = new ApplicationUser { UserName = userName, Email = userName };

            using (var scope = _host.ScopeFactory.CreateScope())
            {
                await scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>()
                    .CreateAsync(user, password);
            }

            return user.Id;
        }
    }
}