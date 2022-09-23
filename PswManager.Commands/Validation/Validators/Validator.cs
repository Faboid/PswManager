using PswManager.Commands.Validation.Models;
using System.Collections.Generic;
using System.Linq;

namespace PswManager.Commands.Validation.Validators; 
internal class Validator<T> : IValidator<T> {

    internal Validator(IReadOnlyCollection<ICondition<T>> conditions, List<IAutoValidator<T>> autoValidator) {
        this.conditions = conditions;
        this.autoValidator = autoValidator;
    }

    readonly IReadOnlyCollection<ICondition<T>> conditions;
    readonly List<IAutoValidator<T>> autoValidator;

    /// <summary>
    /// Runs the given object through a list of prebuild conditions.
    /// </summary>
    /// <param name="obj"></param>
    /// <returns>The errors in string format. If there's none, the validation was successful.</returns>
    public IEnumerable<string> Validate(T obj) {
        if(obj is null) {
            yield return "The given object is null.";
            yield break;
        }

        var errors = autoValidator.Select(x => x.Validate(obj)).SelectMany(x => x).ToList();
        foreach(var err in errors) {
            yield return err;
        }

        List<int> failedConditions = new();
        if(errors.Any()) {
            failedConditions.Add(-1);
        }

        foreach(var cond in conditions) {
            if(!cond.IsValid(obj, failedConditions)) {
                yield return cond.GetErrorMessage();
            }
        }

    }

}
