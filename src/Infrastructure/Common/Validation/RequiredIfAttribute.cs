using System;
using System.ComponentModel.DataAnnotations;

namespace MyWarehouse.Infrastructure.Common.Validation
{
    /// <summary>
    /// Validates a property as required if a boolean flag has a specific value.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class RequiredIfAttribute : RequiredAttribute
    {
        private readonly string _flagName;
        private readonly bool _condition;

        public RequiredIfAttribute(string flagName, bool condition)
        {
            _flagName = flagName;
            _condition = condition;
        }

        protected override ValidationResult IsValid(object value, ValidationContext context)
        {
            object instance = context.ObjectInstance;
            Type type = instance.GetType();

            if (!bool.TryParse(type.GetProperty(_flagName).GetValue(instance)?.ToString(), out bool flagValue))
            {
                throw new InvalidOperationException($"{nameof(RequiredIfAttribute)} can be used only on bool properties.");
            }

            if (flagValue == _condition && (value == null || (value is string s && string.IsNullOrEmpty(s))))
            {
                return new ValidationResult($"Property {context.MemberName} must have a value when {_flagName} is {_condition}");
            }

            return ValidationResult.Success;
        }
    }
}
