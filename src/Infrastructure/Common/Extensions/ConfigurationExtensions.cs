using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace MyWarehouse.Infrastructure;

public static class ConfigurationExtensions
{
    const string ConfigMissingErrorMessage = "Options type of '{0}' was requested as required, but the corresponding section was not found in configuration. Make sure one of your configuration sources contains this section.";

    /// <summary>
    /// Returns part of the configuration, bound to the specified strongly typed options instance. Use this method to get the configuration values needed in startup logic.
    /// </summary>
    /// <typeparam name="T">The options type to bind. The name of this type is used as the section name as well.</typeparam>
    /// <param name="requiredToExistInConfiguration">
    /// If true, the configuration must be backed by an explicitly existing section in the configuration file, and an exception is thrown if not.
    /// If false, a default instance of the configuration type is returned.
    /// </param>
    public static T GetMyOptions<T> (this IConfiguration configuration, bool requiredToExistInConfiguration = false) where T : class, new()
    {
        var bound = configuration.GetSection(typeof(T).Name).Get<T>();

        if (bound is null && requiredToExistInConfiguration)
            throw new InvalidOperationException(string.Format(ConfigMissingErrorMessage, typeof(T).Name));

        bound ??= new T();
        Validator.ValidateObject(bound, new ValidationContext(bound), validateAllProperties: true);

        return bound;
    }

    /// <summary>
    /// Registers a type as a strongly typed configuration, automatically taking care of any data annotation validation on startup, if exists.
    /// The registered type is retrievable from DI directly as a singleton, or wrapped in IOptions<>.
    /// </summary>
    /// <typeparam name="T">The options type to bind. The name of this type is used as the section name as well.</typeparam>
    /// <param name="requiredToExistInConfiguration">
    /// If true, the configuration must be backed by an explicitly existing section in the configuration file, and an exception is thrown if not.
    /// If false, a default instance of the configuration type is returned.
    /// </param>
public static void RegisterMyOptions<T>(this IServiceCollection services, bool requiredToExistInConfiguration = true) where T : class
{
    var optionsBuilder = services.AddOptions<T>()
        .BindConfiguration(typeof(T).Name)
        .ValidateDataAnnotations()
        .ValidateOnStart();

    if (requiredToExistInConfiguration)
        optionsBuilder.Validate<IConfiguration>((_, configuration)
            => configuration.GetSection(typeof(T).Name).Exists(), string.Format(ConfigMissingErrorMessage, typeof(T).Name));

    services.AddSingleton(resolver => resolver.GetRequiredService<IOptions<T>>().Value);
}
}
