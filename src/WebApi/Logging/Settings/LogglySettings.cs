using MyWarehouse.Infrastructure.Common.Validation;
using System.ComponentModel.DataAnnotations;

namespace MyWarehouse.WebApi.Logging.Settings
{
    public class LogglySettings
    {
        [Required]
        public bool? WriteToLoggly { get; init; }

        [RequiredIf(nameof(WriteToLoggly), true)]
        public string CustomerToken { get; init; }
    }
}
