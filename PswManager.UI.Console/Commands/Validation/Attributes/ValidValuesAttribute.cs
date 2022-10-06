using PswManager.Commands.Validation.Attributes;

namespace PswManager.ConsoleUI.Commands.Validation.Attributes;

/// <summary>
/// Provides a set of allowed values.
/// </summary>
public class ValidValuesAttribute : RuleAttribute {

    public string[] ValidValues { get; init; }

    public ValidValuesAttribute(string errorMessage, params string[] validValues) : base(errorMessage) {
        ValidValues = validValues;
    }

}
