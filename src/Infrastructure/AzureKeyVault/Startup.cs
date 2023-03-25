using Azure.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using MyWarehouse.Infrastructure.AzureKeyVault.Settings;
using System.Diagnostics.CodeAnalysis;

namespace MyWarehouse.Infrastructure.AzureKeyVault;

[ExcludeFromCodeCoverage]
internal static class Startup
{
    public static void ConfigureAppConfiguration(HostBuilderContext _, IConfigurationBuilder configBuilder)
    {
        var settings = configBuilder.Build().GetMyOptions<AzureKeyVaultSettings>();

        if (settings is not null && settings.AddToConfiguration)
        {
            configBuilder.AddAzureKeyVault(new Uri(settings.ServiceUrl), new DefaultAzureCredential());
        }
    }
}
