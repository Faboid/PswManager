using PswManager.Commands.Validation.Attributes;
using PswManager.Commands.Validation.Models;
using PswManager.ConsoleUI.Commands.Validation.Attributes;
using System.Linq;

namespace PswManager.ConsoleUI.Commands.Validation.ValidationTypes;

/// <summary>
/// Splits the property string with ' ', then ensures that the split parts do not hold any duplicate.
/// </summary>
public class NoDuplicateValuesRule : ValidationRule {

    public NoDuplicateValuesRule() : base(typeof(NoDuplicateValuesAttribute), typeof(string)) { }

    protected override bool InnerLogic(RuleAttribute attribute, object value) {
        //as this check cares only that there's no duplicates, nulls are a-okay
        if(value == null) {
            return true;
        }

        var values = (value as string).Split(' ').Where(x => !string.IsNullOrWhiteSpace(x));
        return values.Distinct().Count() == values.Count();
    }
}
