using PswManager.Commands.Validation.Attributes;

namespace PswManager.ConsoleUI.Commands.Validation.Attributes; 
public class ValidValuesAttribute : RuleAttribute {

    public string[] ValidValues { get; init; }

    public ValidValuesAttribute(string errorMessage, params string[] validValues) : base(errorMessage) {
        ValidValues = validValues;
    }

}
