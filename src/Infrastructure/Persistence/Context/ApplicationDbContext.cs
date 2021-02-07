using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MyWarehouse.Application.Dependencies.Services;
using MyWarehouse.Domain.Common;
using MyWarehouse.Domain.Partners;
using MyWarehouse.Domain.Products;
using MyWarehouse.Domain.Transactions;
using MyWarehouse.Infrastructure.Identity.Model;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyWarehouse.Domain.Common.ValueObjects.Money;
using MyWarehouse.Domain.Common.ValueObjects.Mass;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace MyWarehouse.Infrastructure.Persistence.Context
{
    // In this particular architecture, AppDbContext only contains
    // translation logic required for proper loading and saving of
    // domain entities. Query-specific customizations and utility
    // logic has been relegated higher, to repositories.

    /// <summary>
    /// DB context implementation for Entity Framework.
    /// </summary>
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<Product> Products { get; set; }
        public DbSet<Partner> Partners { get; set; }
        public DbSet<Transaction> Transactions { get; set; }

        private readonly ICurrentUserService _currentUser;
        private readonly IDateTime _dateTime;

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, ICurrentUserService currentUser, IDateTime dateTime) : base(options)
        {
            _currentUser = currentUser;
            _dateTime = dateTime;

            // The default cascade delete, combined with the default Immediate cascade timing setting,
            // can cause problems if we intend to manually revert the state of EntityEntries.
            // This is because deletion state change cascades to navigation properties, even owned types,
            // and reverting the state change on the root entity doesn't revert the cascaded changes.
            // This is particularly problematic for owned types, where it seems EF incorrectly
            // does a cascade and write nulls to the DB, even when the owned type is non-nullable,
            // causing errors when saving entities.
            // Here I effectively disabled the cascade for then-reverted (soft-deleted) entities.
            // But in a production system the cascade behavior has to be properly designed.
            ChangeTracker.CascadeDeleteTiming = CascadeTiming.OnSaveChanges;
        }

        // Added for LinqPad.
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public override int SaveChanges()
            => SaveChanges(acceptAllChangesOnSuccess: true);

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            ApplyMyEntityOverrides();
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
            => SaveChangesAsync(acceptAllChangesOnSuccess: true, cancellationToken);

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            ApplyMyEntityOverrides();
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            ConfigureValueObjects(builder);
            ConfigureDecimalPrecision(builder);
            ConfigureSoftDeleteFilter(builder);

            base.OnModelCreating(builder);
        }

        /// <summary>
        /// Configure the value object of the application domain as EF Core owned types.
        /// </summary>
        private static void ConfigureValueObjects(ModelBuilder builder)
        {
            builder.Owned(typeof(Money));
            builder.Owned(typeof(Currency));
            builder.Owned(typeof(Address));
            builder.Owned(typeof(Mass));
            builder.Owned(typeof(MassUnit));

            // Unfortunately, as of EF Core 5.0, there is no way to centrally configure owned types.
            // All references to the owned types have to be configured individually.

            // Store and restore mass unit as symbol.
            builder.Entity<Product>().OwnsOne(p => p.Mass, StoreMassUnitAsSymbol);

            // Store and restore currency as currency code.
            builder.Entity<Product>().OwnsOne(p => p.Price, StoreCurrencyAsCode);
            builder.Entity<Transaction>().OwnsOne(p => p.Total, StoreCurrencyAsCode);
            builder.Entity<TransactionLine>().OwnsOne(p => p.UnitPrice, StoreCurrencyAsCode);

            static void StoreCurrencyAsCode<T>(OwnedNavigationBuilder<T, Money> onb) where T : class
                => onb.Property(m => m.Currency)
                    .HasConversion(
                        c => c.Code,
                        c => Currency.FromCode(c))
                    .HasMaxLength(3);

            static void StoreMassUnitAsSymbol<T>(OwnedNavigationBuilder<T, Mass> onb) where T : class
                => onb.Property(m => m.Unit)
                    .HasConversion(
                        u => u.Symbol,
                        s => MassUnit.FromSymbol(s))
                    .HasMaxLength(3);
        }

        /// <summary>
        /// Set all decimal properties to a custom uniform precision.
        /// </summary>
        private static void ConfigureDecimalPrecision(ModelBuilder builder)
        {
            foreach (var entityType in builder.Model.GetEntityTypes())
            {
                foreach (var decimalProperty in entityType.GetProperties()
                    .Where(x => x.ClrType == typeof(decimal)))
                {
                    decimalProperty.SetPrecision(18);
                    decimalProperty.SetScale(4);
                }
            }
        }

        /// <summary>
        /// Set global filter on all soft-deletable entities to exclude the ones which are 'deleted'.
        /// </summary>
        private static void ConfigureSoftDeleteFilter(ModelBuilder builder)
        {
            foreach (var softDeletableTypeBuilder in builder.Model.GetEntityTypes()
                .Where(x => typeof(ISoftDeletable).IsAssignableFrom(x.ClrType)))
            {
                var parameter = Expression.Parameter(softDeletableTypeBuilder.ClrType, "p");

                softDeletableTypeBuilder.SetQueryFilter(
                    Expression.Lambda(
                        Expression.Equal(
                            Expression.Property(parameter, nameof(ISoftDeletable.DeletedAt)),
                            Expression.Constant(null)),
                        parameter)
                );
            }
        }

        /// <summary>
        /// Automatically stores metadata when entities are added, modified, or deleted.
        /// </summary>
        private void ApplyMyEntityOverrides()
        {
            foreach (var entry in ChangeTracker.Entries<IAudited>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Property(nameof(IAudited.CreatedBy)).CurrentValue = _currentUser.UserId;
                        entry.Property(nameof(IAudited.CreatedAt)).CurrentValue = _dateTime.Now;
                        break;
                    case EntityState.Modified:
                        entry.Property(nameof(IAudited.LastModifiedBy)).CurrentValue = _currentUser.UserId;
                        entry.Property(nameof(IAudited.LastModifiedAt)).CurrentValue = _dateTime.Now;
                        break;
                }
            }

            foreach (var entry in ChangeTracker.Entries<ISoftDeletable>())
            {
                switch (entry.State)
                {
                    case EntityState.Deleted:
                        entry.State = EntityState.Unchanged; // Override removal. Better than Modified, because that flags ALL properties for update.
                        entry.Property(nameof(ISoftDeletable.DeletedBy)).CurrentValue = _currentUser.UserId;
                        entry.Property(nameof(ISoftDeletable.DeletedAt)).CurrentValue = _dateTime.Now;
                        break;
                }
            }
        }
    }
}