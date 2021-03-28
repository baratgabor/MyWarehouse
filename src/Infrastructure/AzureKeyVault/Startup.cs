using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.AzureKeyVault;
using Microsoft.Extensions.Hosting;
using MyWarehouse.Infrastructure.AzureKeyVault.Settings;
using System.Diagnostics.CodeAnalysis;

namespace MyWarehouse.Infrastructure.AzureKeyVault
{
    [ExcludeFromCodeCoverage]
    internal static class Startup
    {
        public static void ConfigureAppConfiguration(HostBuilderContext _, IConfigurationBuilder configBuilder)
        {
            var settings = configBuilder.Build().GetMyOptions<AzureKeyVaultSettings>();

            if (settings.AddToConfiguration)
            {
                configBuilder.AddAzureKeyVault(
                    settings.ServiceUrl,
                    new KeyVaultClient(
                        new KeyVaultClient.AuthenticationCallback(
                            new AzureServiceTokenProvider().KeyVaultTokenCallback)),
                    new DefaultKeyVaultSecretManager()
                );
            }
        }
    }
}