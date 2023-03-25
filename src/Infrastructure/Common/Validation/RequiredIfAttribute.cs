namespace MyWarehouse.Infrastructure.Common.Validation;

/// <summary>
/// Validates a property as required if a boolean flag has a specific value.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class RequiredIfAttribute : RequiredAttribute
{
    private readonly string _otherPropertyName;
    private readonly bool _otherPropertyValue;

    public RequiredIfAttribute(string otherPropertyName, bool otherPropertyValue)
    {
        _otherPropertyName = otherPropertyName;
        _otherPropertyValue = otherPropertyValue;
        AllowEmptyStrings = false;
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext context)
    {
        if (IsValidationRequired(context)) return base.IsValid(value, context);

        return ValidationResult.Success;
    }

    /// <summary>
    /// Checks if the specified other property has the specified value, making the 'Required' validation necessary.
    /// </summary>
    private bool IsValidationRequired(ValidationContext context)
    {
        var otherProperty = context.ObjectInstance.GetType().GetProperty(_otherPropertyName);

        if (otherProperty is null)
            throw new ArgumentException($"The specified property '{_otherPropertyName}' is not found on the validatable object.");

        if (otherProperty.PropertyType != typeof(bool) && otherProperty.PropertyType != typeof(bool?))
            throw new ArgumentException($"The specified property '{_otherPropertyName}' on the validatable object must be of type bool.");

        var ifConditionSatisfied = otherProperty.GetValue(context.ObjectInstance) as bool? == _otherPropertyValue;

        return ifConditionSatisfied;
    }
}
