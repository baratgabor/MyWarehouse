namespace MyWarehouse.WebApi.CORS
{
    // Used for binding allowed origins in a strongly typed manner
    // from JSON configuration.
    public class CorsSettings
    {
        public string[] AllowedOrigins { get; init; }
    }
}
