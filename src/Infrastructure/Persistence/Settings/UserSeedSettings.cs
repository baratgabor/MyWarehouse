using MyWarehouse.Infrastructure.Common.Validation;

namespace MyWarehouse.Infrastructure.Persistence.Settings
{
    class UserSeedSettings
    {
        public bool SeedDefaultUser { get; init; }
        
        [RequiredIf(nameof(SeedDefaultUser), true)]
        public string DefaultUsername { get; init; }

        [RequiredIf(nameof(SeedDefaultUser), true)]
        public string DefaultPassword { get; init; }

        public string DefaultEmail { get; init; }
    }
}
