using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MyWarehouse.Application.Common.Dependencies.DataAccess;
using MyWarehouse.Application.Common.Dependencies.DataAccess.Repositories;
using MyWarehouse.Application.Dependencies.Services;
using MyWarehouse.Infrastructure.ApplicationDependencies.DataAccess;
using MyWarehouse.Infrastructure.ApplicationDependencies.DataAccess.Repositories;
using MyWarehouse.Infrastructure.ApplicationDependencies.Services;

namespace MyWarehouse.Infrastructure.ApplicationDependencies
{
    public static class Startup
    {
        public static void ConfigureServices(this IServiceCollection services, IConfiguration _)
        {
            services.AddScoped<IProductRepository, ProductRepositoryEF>();
            services.AddScoped<IPartnerRepository, PartnerRepositoryEF>();
            services.AddScoped<ITransactionRepository, TransactionRepositoryEF>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddTransient<IDateTime, DateTimeService>();
            services.AddTransient<IStockStatisticsService, StockStatisticsService>();
        }
    }
}
