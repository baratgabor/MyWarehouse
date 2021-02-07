using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;
using MyWarehouse.Application.Dependencies.Services;
using System.IO;
using System.Reflection;

namespace MyWarehouse.Application.IntegrationTests
{
    public class TestHost
    {
        public string ConnectionString { get; private set; }
        public IWebHostEnvironment Environment { get; private set; }
        public IServiceScopeFactory ScopeFactory { get; private set; }
        public string CurrentUserId => FakeCurrentUserService.DefaultUserId;

        private TestHost() { }

        public static TestHost Create() => new TestHost().SetUpHost();

        private TestHost SetUpHost()
        {
            Environment = Mock.Of<IWebHostEnvironment>(w =>
                w.EnvironmentName == "Development" &&
                w.ApplicationName == "MyWarehouse.Api");

            // Last added overrides. Make sure .Testing.json is placed after API jsons.
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
                .AddUserSecrets(Assembly.Load(new AssemblyName(Environment.ApplicationName)), optional: true)
                .AddJsonFile("appsettings.Testing.json", true, true)
                .AddEnvironmentVariables()
                .Build();

            var services = new ServiceCollection()
                .AddSingleton(Environment)
                .AddLogging();

            new MyWarehouse.Startup(configuration, Environment)
                .ConfigureServices(services);

            services = ReplaceServices(services);

            ScopeFactory = services.BuildServiceProvider().GetService<IServiceScopeFactory>();
            ConnectionString = configuration.GetConnectionString("DefaultConnection");
            
            return this;
        }

        // Anticipating the need to replace multiple services in near future.
        private static IServiceCollection ReplaceServices(IServiceCollection services)
        {
            services.Replace(ServiceDescriptor.Transient<ICurrentUserService, FakeCurrentUserService>());

            return services;
        }

        private class FakeCurrentUserService : ICurrentUserService
        {
            public static readonly string DefaultUserId = "Test";
            public string UserId => DefaultUserId;
        }
    }
}