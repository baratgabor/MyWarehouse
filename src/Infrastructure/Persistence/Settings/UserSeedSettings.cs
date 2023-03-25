using MyWarehouse.Infrastructure.Common.Validation;
using System.Diagnostics.CodeAnalysis;

namespace MyWarehouse.Infrastructure.Persistence.Settings;

class UserSeedSettings
{
    [MemberNotNullWhen(true, nameof(DefaultUsername), nameof(DefaultPassword))]
    public bool SeedDefaultUser { get; init; }
    
    [RequiredIf(nameof(SeedDefaultUser), true)]
    public string? DefaultUsername { get; init; }

    [RequiredIf(nameof(SeedDefaultUser), true)]
    public string? DefaultPassword { get; init; }

    public string DefaultEmail { get; init; } = null!;
}
