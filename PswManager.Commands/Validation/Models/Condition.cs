using System;
using System.Collections.Generic;
using System.Linq;

namespace PswManager.Commands.Validation.Models {
    public class Condition<T> : ICondition<T> {

        public Condition(IndexHelper index, Func<T, bool> logic, string errorMessage) {
            Index = index;
            Logic = logic;
            ErrorMessage = errorMessage;
        }

        private readonly string ErrorMessage;
        public IndexHelper Index { get; init; }
        public Func<T, bool> Logic { get; init; }

        public string GetErrorMessage() => ErrorMessage;

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
            }
            catch {
                return false;
            }

            failedConditions.Add(Index.Index);
            return false;
        }

    }
}
