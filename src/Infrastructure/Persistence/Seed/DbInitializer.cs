using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MyWarehouse.Domain.Partners;
using MyWarehouse.Domain.Products;
using MyWarehouse.Infrastructure.Identity.Model;
using MyWarehouse.Infrastructure.Persistence.Context;
using MyWarehouse.Infrastructure.Persistence.Settings;
using MyWarehouse.TestData;

namespace MyWarehouse.Infrastructure.Persistence.Seed;

static class DbInitializer
{
    public static void SeedDatabase(IApplicationBuilder appBuilder, IConfiguration configuration)
    {
        using (var scope = appBuilder.ApplicationServices.CreateScope())
        {
            var services = scope.ServiceProvider;
            var settings = configuration.GetMyOptions<ApplicationDbSettings>();

            try
            {
                var context = services.GetRequiredService<ApplicationDbContext>();

                if (settings.AutoMigrate == true && context.Database.IsSqlServer())
                {
                    context.Database.Migrate();
                }

                if (settings.AutoSeed == true)
                {
                    SeedDefaultUser(services, configuration.GetMyOptions<UserSeedSettings>());
                    SeedSampleData(context);
                }
            }
            catch (Exception exception)
            {
                var logger = scope.ServiceProvider.GetRequiredService<ILogger<ApplicationDbContext>>();

                logger.LogError(exception, "An error occurred while migrating or seeding the database.");

                throw;
            }
        }
    }

    private static void SeedDefaultUser(IServiceProvider services, UserSeedSettings settings)
    {
        if (!settings.SeedDefaultUser)
            return;

        using (var userManager = services.GetRequiredService<UserManager<ApplicationUser>>())
        {
            if (!userManager.Users.Any(u => u.UserName == settings.DefaultUsername))
            {
                var defaultUser = new ApplicationUser { UserName = settings.DefaultUsername, Email = settings.DefaultEmail };
                var result = userManager.CreateAsync(defaultUser, settings.DefaultPassword).GetAwaiter().GetResult();

                if (!result.Succeeded)
                {
                    throw new Exception($"Default user creation failed with the following errors: "
                        + result.Errors.Aggregate("", (sum, err) => sum += $"{Environment.NewLine} - {err.Description}"));
                }
            }
        }
    }

    private static void SeedSampleData(ApplicationDbContext context)
    {
        if (!context.Partners.Any() && !context.Products.Any())
        {
            var (products, partners) = DataGenerator.GenerateBaseEntities();
            
            context.Partners.AddRange(partners);
            context.Products.AddRange(products);
            context.SaveChanges();
        }

        if (!context.Transactions.Any())
        {
            var products = context.Products.ToList();
            var partners = context.Partners.ToList();

            // This is the best way to add transactions; to save after each one.
            // Trust me, I tried. Batch saving causes them to get 'grouped' by partners via their nav property.
            // Then, when you list transactions by ID, you get a bunch for the same partner one after each other.
            // And trying to solve it via AsNoTracking(), changetracker.Clear(), Reflection, etc. is a nightmare.
            var transactionsToGenerate = 103;
            for (int i = 0; i < transactionsToGenerate; i++)
            {
                context.Transactions.Add(
                    DataGenerator.GenerateTransaction(partners, products));
                context.SaveChanges();
            }
        }
    }
}
