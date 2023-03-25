using MyWarehouse.Infrastructure.Common.Validation;
using System.Diagnostics.CodeAnalysis;

namespace MyWarehouse.Infrastructure.AzureKeyVault.Settings;

internal class AzureKeyVaultSettings
{
    [RequiredIf(nameof(AddToConfiguration), true)]
    public string? ServiceUrl { get; init; }
    
    [MemberNotNullWhen(true, nameof(ServiceUrl))]
    public bool AddToConfiguration { get; init; }
}
