using PswManager.Commands.Validation.Attributes;

namespace PswManager.ConsoleUI.Commands.Validation.Attributes;
public class NoDuplicateValuesAttribute : RuleAttribute {

    public NoDuplicateValuesAttribute(string errorMessage) : base(errorMessage) {

    }

}
