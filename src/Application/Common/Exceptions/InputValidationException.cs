using FluentValidation.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace MyWarehouse.Application.Common.Exceptions
{
    // TODO: Replace throwing exceptions for validation with an app-wide result model that contains operation result.
    public class InputValidationException : Exception
    {
        public IDictionary<string, string[]> Errors { get; }

        public InputValidationException(Exception innerException = null)
            : base("One or more validation failures have occurred.", innerException)
        {
            Errors = new Dictionary<string, string[]>();
        }

        public InputValidationException(params (string PropertyName, string ErrorMessage)[] failures) : this()
        {
            var failureGroups = failures
                .GroupBy(e => e.PropertyName, e => e.ErrorMessage);

            foreach (var failureGroup in failureGroups)
            {
                var propertyName = failureGroup.Key;
                var propertyFailures = failureGroup.ToArray();

                Errors.Add(propertyName, propertyFailures);
            }
        }

        public InputValidationException(Exception innerException, params (string PropertyName, string ErrorMessage)[] failures) : this(innerException)
        {
            var failureGroups = failures
                .GroupBy(e => e.PropertyName, e => e.ErrorMessage);

            foreach (var failureGroup in failureGroups)
            {
                var propertyName = failureGroup.Key;
                var propertyFailures = failureGroup.ToArray();

                Errors.Add(propertyName, propertyFailures);
            }
        }

        public InputValidationException(IEnumerable<ValidationFailure> failures)
            : this()
        {
            var failureGroups = failures
                .GroupBy(e => e.PropertyName, e => e.ErrorMessage);

            foreach (var failureGroup in failureGroups)
            {
                var propertyName = failureGroup.Key;
                var propertyFailures = failureGroup.ToArray();

                Errors.Add(propertyName, propertyFailures);
            }
        }

        public override string ToString()
        {
            return nameof(InputValidationException) + ": " + JsonSerializer.Serialize(this, new JsonSerializerOptions() { WriteIndented = true });
        }
    }
}
