using System.ComponentModel.DataAnnotations;

namespace MyWarehouse.WebApi.Logging.Settings
{
    public class LogglySettings
    {
        [Required]
        public bool? WriteToLoggly { get; init; }

        [Required, MinLength(1)]
        public string CustomerToken { get; init; }
    }
}
