using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PswManagerCommands.Validation {
    public class ValidatorBuilder<T> {

        readonly List<Condition<T>> conditions = new();
        AutoValidation<T> autoValidator;

        public ValidatorBuilder<T> AddCondition(IndexHelper index, Func<T, bool> conditionFunction, string errorMessage) {
            conditions.Add(new Condition<T>(index, conditionFunction, errorMessage));
            return this;
        }

        public ValidatorBuilder<T> AddCondition(Condition<T> condition) {
            conditions.Add(condition);
            return this;
        }

        public ValidatorBuilder<T> AddAutoValidator(AutoValidation<T> autoValidator) {
            this.autoValidator = autoValidator;
            return this;
        }

        public Validator<T> Build() {
            return new Validator<T>(conditions, autoValidator);
        }

    }

    public class Condition<T> {

        public Condition(IndexHelper index, Func<T, bool> logic, string errorMessage) {
            Index = index;
            Logic = logic;
            ErrorMessage = errorMessage;
        }

        public IndexHelper Index { get; init; }
        public Func<T, bool> Logic { get; init; }
        public string ErrorMessage { get; init; }

        public bool IsValid(T obj) => IsValid(obj, new List<int>());
        public bool IsValid(T obj, IList<int> failedConditions) {

            //if a required condition has failed, return true
            //reason being that it's unecessary to pile the user with unnecessary error messages
            if(Index.RequiredSuccesses.Any(x => failedConditions.Contains(x))) {
                return true;
            }

            try {

                //if the validation passes, return true
                if(Logic.Invoke(obj)) {
                    return true;
                }
            } catch {
                return false;
            }

            failedConditions.Add(Index.Index);
            return false;
        }

    }

    public class Validator<T> {

        internal Validator(IReadOnlyCollection<Condition<T>> conditions, AutoValidation<T> autoValidator) {
            this.conditions = conditions;
            this.autoValidator = autoValidator;
        }

        readonly IReadOnlyCollection<Condition<T>> conditions;
        readonly AutoValidation<T> autoValidator;

        /// <summary>
        /// Runs the given object through a list of prebuild conditions.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns>The errors in string format. If there's none, the validation was successful.</returns>
        public IEnumerable<string> Validate(T obj) {
            var errors = autoValidator?.Validate(obj) ?? Enumerable.Empty<string>();
            foreach(var err in errors) {
                yield return err;
            }

            List<int> failedConditions = new();
            if(errors.Any()) {
                failedConditions.Add(-1);
            }

            foreach(var cond in conditions) {
                if(!cond.IsValid(obj, failedConditions)) {
                    yield return cond.ErrorMessage;
                }
            }

        }

    }

}
